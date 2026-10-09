using EarlyStep.BLL.Interfaces;
using EarlyStep.Contracts.DTOS.Requests.AccountRequests;
using EarlyStep.Contracts.DTOS.Responses.AccountResponses;
using EarlyStep.DAL.Data;
using EarlyStep.DAL.Model;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.EntityFrameworkCore;
using System.Text;

namespace EarlyStep.BLL.Repository;

public class AuthRepository : IAuthRepository
    {
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly SignInManager<ApplicationUser> _signInManager;
    private readonly ITokenService _tokenService;
    private readonly RoleManager<ApplicationRole> _roleManager;
    private readonly ApplicationDbContext _dbContext;



    public AuthRepository(
        UserManager<ApplicationUser> userManager,
        SignInManager<ApplicationUser> signInManager,
        RoleManager<ApplicationRole> roleManager,
        ITokenService tokenService,
        ApplicationDbContext dbContext)
        {
        _userManager = userManager;
        _signInManager = signInManager;
        _tokenService = tokenService;
        _roleManager = roleManager;
        _dbContext = dbContext;

        }
    public async Task<RegisterResponse?> RegisterAsync(
        RegisterRequests register,
        CancellationToken cancellationToken = default)
        {
        if (register.Password != register.ConfirmPassword)
            return null;

        var exists = await _userManager.FindByEmailAsync(register.Email);

        if (exists != null)
            return null;

        const string defaultRole = "ولي أمر";

        var user = new ApplicationUser
            {
            FullName = register.FullName,
            Email = register.Email,
            UserName = register.Email
            };

        var result = await _userManager.CreateAsync(
            user,
            register.Password);

        if (!result.Succeeded)
            {
            var errors = result.Errors
                .Select(e => e.Description);

            throw new Exception(string.Join(", ", errors));
            }

        // التأكد من وجود دور ولي أمر
        if (!await _roleManager.RoleExistsAsync(defaultRole))
            {
            var roleResult = await _roleManager.CreateAsync(
                new ApplicationRole
                    {
                    Name = defaultRole,
                    NormalizedName = defaultRole.ToUpperInvariant()
                    });

            if (!roleResult.Succeeded)
                {
                var errors = roleResult.Errors
                    .Select(e => e.Description);

                throw new Exception(string.Join(", ", errors));
                }
            }

        // إضافة المستخدم تلقائيًا إلى دور ولي أمر
        var addToRoleResult = await _userManager.AddToRoleAsync(
            user,
            defaultRole);

        if (!addToRoleResult.Succeeded)
            {
            var errors = addToRoleResult.Errors
                .Select(e => e.Description);

            throw new Exception(string.Join(", ", errors));
            }

        return new RegisterResponse
            {
            Message = "تم إنشاء الحساب بنجاح.",
            Role = defaultRole
            };
        }

    public async Task<AuthResponse?> LoginAsync(
        LoginRequests login,
        CancellationToken cancellationToken = default)
        {
        var user = await _userManager.FindByEmailAsync(login.Email);

        if (user == null)
            return null;

        if (!user.IsActive)
            return null;

        if (!user.EmailConfirmed)
            throw new Exception("Please confirm your email first.");

        var result = await _signInManager.CheckPasswordSignInAsync(
            user,
            login.Password,
            lockoutOnFailure: false);

        if (!result.Succeeded)
            return null;

        var roles = await _userManager.GetRolesAsync(user);

        var response = await _tokenService.CreateTokenAsync(user);

        if (response == null)
            return null;

        response.Role = roles.FirstOrDefault();

        return response;
        }

    public async Task<IdentityResult> ConfirmEmailAsync(
            string userId,
            string token,
            CancellationToken cancellationToken = default)
        {
        var user = await _userManager.FindByIdAsync(userId);

        if (user == null)
            {
            return IdentityResult.Failed(
                new IdentityError
                    {
                    Description = "User not found."
                    });
            }

        var decodedToken =
            Encoding.UTF8.GetString(WebEncoders.Base64UrlDecode(token));

        return await _userManager.ConfirmEmailAsync(user, decodedToken);
        }


    public async Task<IdentityResult> ChangePasswordAsync(
        string userId,
        ChangePasswordRequest request,
        CancellationToken cancellationToken = default)
        {
        if (request.NewPassword != request.ConfirmPassword)
            return IdentityResult.Failed(new IdentityError { Description = "كلمتا المرور الجديدتين غير متطابقتين." });

        var user = await _userManager.FindByIdAsync(userId);
        if (user == null)
            return IdentityResult.Failed(new IdentityError { Description = "المستخدم غير موجود." });

        return await _userManager.ChangePasswordAsync(user, request.CurrentPassword, request.NewPassword);
        }
    public async Task<string?> ForgotPassword(ForgotPasswordRequest request, CancellationToken cancellationToken = default)
        {
        var user = await _userManager.FindByEmailAsync(request.Email);
        if (user == null) return null;

        var code = new Random().Next(1000, 9999).ToString();

        var resetCode = new PasswordResetCode
            {
            Code = code,
            ExpirationCode = DateTime.Now.AddMinutes(15),
            UserId = user.Id
            };
        _dbContext.passwordRestCodes.Add(resetCode);
        await _dbContext.SaveChangesAsync(cancellationToken);
        return code;
        }


    public async Task<IdentityResult> ResetPasswordAsync(ResetPasswordRequest req, CancellationToken ct = default)
        {
        if (req.NewPassword != req.ConfirmPassword)
            return IdentityResult.Failed(new IdentityError { Description = "كلمتا المرور الجديدتين غير متطابقتين." });


        var user = await _userManager.FindByEmailAsync(req.Email);
        if (user is null)
            return IdentityResult.Failed(new IdentityError { Description = "User not found." });

        var now = DateTime.Now;
        //  هس هون بدي اجيب اليوزر الي بعتله الكود بالايميل وانه الكود الي بعته بالايميل هو نفس الكود الي دخله اليوزر 
        //+ انه الكود ما استخدمه ووقته ما خلص
        var codeRow = await _dbContext.passwordRestCodes
            .Where(c => c.UserId == user.Id &&
                        c.Code == req.Code &&
                        !c.IsUsed &&
                        c.ExpirationCode > now)
            .OrderByDescending(c => c.Created)
            .FirstOrDefaultAsync(ct);

        if (codeRow is null)
            return IdentityResult.Failed(new IdentityError { Description = "Invalid or expired code." });

        // هون بدي ابلش ترانزاكشن لانه ممكن اليوزر يكون عنده باسورد وانا بدي احذف الباسورد القديم و اضيف الباسورد الجديد
        // بدي اشوف اذا اليوزر عنده باسورد قديم اذا  كان عنده بدي احذفه واحفظ الباسورد الجديد
        // اذا ما كان عنده باسورد قديم بدي اضيف الباسورد الجديد
        //إذا حصل خطأ أثناء إزالة الباسورد القديم أو إضافة الباسورد الجديد، يتم التراجع عن أي تغييرات.

        var strategy = _dbContext.Database.CreateExecutionStrategy();

        return await strategy.ExecuteAsync(async () =>
        {
            using var tx = await _dbContext.Database.BeginTransactionAsync(ct);

            var hasPassword = await _userManager.HasPasswordAsync(user);
            if (hasPassword)
                {
                var remove = await _userManager.RemovePasswordAsync(user);
                if (!remove.Succeeded) return remove;
                }
            // هلا بدي اضيف الباسورد الجديد + احدث الكود انه استعمل والوقت الي استعمل فيه

            var add = await _userManager.AddPasswordAsync(user, req.NewPassword);
            if (!add.Succeeded) return add;

            codeRow.IsUsed = true;
            codeRow.UsedUtc = now;

            await _dbContext.SaveChangesAsync(ct);
            await tx.CommitAsync(ct);

            return IdentityResult.Success;
        });
        }

    }