using EarlyStep.DAL.Model;
using Microsoft.AspNetCore.Identity;

namespace EarlyStep.DAL.Data;

public static class IdentitySeeder
    {
    public static async Task SeedAsync(
        UserManager<ApplicationUser> userManager,
        RoleManager<ApplicationRole> roleManager)
        {

        string[] roles =
        {
    "SuperAdmin",
    "Admin",
    "معلم/ة",
    "أخصائي/ة"
};

        foreach (var roleName in roles)
            {
            if (!await roleManager.RoleExistsAsync(roleName))
                {
                await roleManager.CreateAsync(
                    new ApplicationRole
                        {
                        Id = Guid.NewGuid(),
                        Name = roleName,
                        NormalizedName = roleName.ToUpper()
                        });
                }
            }


        // ==========================================
        // Super Admin
        // ==========================================

        await CreateUserAsync(
            userManager,
            fullName: "System Super Admin",
            userName: "superadmin",
            email: "superadmin@earlystep.com",
            password: "SuperAdmin@123",
            role: "SuperAdmin"
        );


        // ==========================================
        // Admin
        // ==========================================

        await CreateUserAsync(
            userManager,
            fullName: "System Admin",
            userName: "admin",
            email: "admin@earlystep.com",
            password: "Admin@123",
            role: "Admin"
        );
        // ==========================================
        // Teachers
        // ==========================================

        await CreateUserAsync(
            userManager,
            fullName: "Teacher One",
            userName: "teacher1",
            email: "teacher1@earlystep.com",
            password: "Teacher@123",
            role: "معلم/ة"
        );

        await CreateUserAsync(
            userManager,
            fullName: "Teacher Two",
            userName: "teacher2",
            email: "teacher2@earlystep.com",
            password: "Teacher@123",
            role: "معلم/ة"
        );


        // ==========================================
        // Specialists
        // ==========================================

        await CreateUserAsync(
            userManager,
            fullName: "Specialist One",
            userName: "specialist1",
            email: "specialist1@earlystep.com",
            password: "Specialist@123",
            role: "أخصائي/ة"
        );

        await CreateUserAsync(
            userManager,
            fullName: "Specialist Two",
            userName: "specialist2",
            email: "specialist2@earlystep.com",
            password: "Specialist@123",
            role: "أخصائي/ة"
        );
        }



    private static async Task CreateUserAsync(
        UserManager<ApplicationUser> userManager,
        string fullName,
        string userName,
        string email,
        string password,
        string role)
        {
        var existingUser = await userManager.FindByEmailAsync(email);

        if (existingUser != null)
            {
            // Make sure the existing user has the role
            if (!await userManager.IsInRoleAsync(existingUser, role))
                {
                await userManager.AddToRoleAsync(
                    existingUser,
                    role);
                }

            return;
            }


        var user = new ApplicationUser
            {
            Id = Guid.NewGuid(),

            FullName = fullName,

            UserName = userName,

            Email = email,

            EmailConfirmed = true,

            IsActive = true,

            CreatedAt = DateTime.UtcNow
            };


        var result = await userManager.CreateAsync(
            user,
            password);


        if (!result.Succeeded)
            {
            var errors = string.Join(
                ", ",
                result.Errors.Select(x => x.Description));

            throw new Exception(
                $"Failed to create {role}: {errors}");
            }


        await userManager.AddToRoleAsync(
            user,
            role);
        }
    }