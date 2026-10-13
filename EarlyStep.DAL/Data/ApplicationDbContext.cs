using EarlyStep.DAL.Model;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace EarlyStep.DAL.Data;

public class ApplicationDbContext : IdentityDbContext<ApplicationUser, ApplicationRole, Guid>
    {
    public ApplicationDbContext(
        DbContextOptions<ApplicationDbContext> options)
        : base(options)
        {

        }


    public DbSet<Child> Children { get; set; }

    public DbSet<Activity> Activities { get; set; }
    public DbSet<ActivityQuestion> ActivityQuestions { get; set; }

    public DbSet<Category> Categories { get; set; }

    public DbSet<InitialAssessment> InitialAssessments { get; set; }
    public DbSet<InitialAssessmentConfig> InitialAssessmentConfigs { get; set; }
    public DbSet<InitialAssessmentSelectedQuestion> InitialAssessmentSelectedQuestions { get; set; }
    public DbSet<InitialAssessmentQuestionResult> InitialAssessmentQuestionResults { get; set; }
    public DbSet<PasswordResetCode> passwordRestCodes { get; set; }

    public DbSet<QuestionOption> QuestionOptions { get; set; }
    public DbSet<Report> Reports { get; set; }
    public DbSet<TrainingPlanFeedback> TrainingPlanFeedback { get; set; }


    public DbSet<TrainingPlan> TrainingPlans { get; set; }

    public DbSet<TrainingPlanActivity> TrainingPlanActivities { get; set; }

    public DbSet<Conversation> Conversations { get; set; }

    public DbSet<Message> Messages { get; set; }

    public DbSet<TrainingPlanQuestionAnswer> TrainingPlanQuestionAnswers { get; set; }

    protected override void OnModelCreating(ModelBuilder builder)
        {
        base.OnModelCreating(builder);

        builder.ApplyConfigurationsFromAssembly(
            typeof(ApplicationDbContext).Assembly);


        builder.Entity<Child>()
            .HasOne(c => c.Parent)
            .WithMany(u => u.ParentChildren)
            .HasForeignKey(c => c.ParentId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.Entity<Child>()
            .HasOne(c => c.Specialist)
            .WithMany(u => u.SpecialistChildren)
            .HasForeignKey(c => c.SpecialistId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.Entity<Child>()
    .HasOne(c => c.Teacher)
    .WithMany()
    .HasForeignKey(c => c.TeacherId)
    .OnDelete(DeleteBehavior.SetNull);

        builder.Entity<Child>()
            .Property(c => c.Gender)
            .HasConversion<string>();


        builder.Entity<Activity>()
                .HasOne(a => a.Category)
                .WithMany(c => c.Activities)
                .HasForeignKey(a => a.CategoryId)
                .OnDelete(DeleteBehavior.Restrict);

        builder.Entity<Activity>()
            .HasIndex(a => a.Slug)
            .IsUnique();


        builder.Entity<InitialAssessment>()
            .HasOne(ia => ia.Child)
            .WithOne(c => c.InitialAssessment)
            .HasForeignKey<InitialAssessment>(ia => ia.ChildId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.Entity<TrainingPlan>()
            .HasOne(tp => tp.Child)
            .WithMany(c => c.TrainingPlans)
            .HasForeignKey(tp => tp.ChildId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.Entity<TrainingPlan>()
            .HasOne(tp => tp.Specialist)
            .WithMany(u => u.TrainingPlansAsSpecialist)
            .HasForeignKey(tp => tp.SpecialistId)
            .OnDelete(DeleteBehavior.Restrict);
        builder.Entity<TrainingPlan>()
            .HasOne(tp => tp.Teacher)
            .WithMany(u => u.TrainingPlansAsTeacher)
            .HasForeignKey(tp => tp.TeacherId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.Entity<TrainingPlanActivity>()
            .HasOne(tpa => tpa.TrainingPlan)
            .WithMany(tp => tp.Activities)
            .HasForeignKey(tpa => tpa.TrainingPlanId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.Entity<TrainingPlanActivity>()
            .HasOne(tpa => tpa.Activity)
            .WithMany(a => a.TrainingPlanActivities)
            .HasForeignKey(tpa => tpa.ActivityId)
            .OnDelete(DeleteBehavior.Restrict);
        builder.Entity<Report>()
            .HasOne(r => r.Child)
            .WithMany(c => c.Reports)
            .HasForeignKey(r => r.ChildId)
            .OnDelete(DeleteBehavior.Cascade);
        builder.Entity<TrainingPlanQuestionAnswer>()
            .HasOne(a => a.TrainingPlanActivity)
            .WithMany(a => a.QuestionAnswers)
            .HasForeignKey(a => a.TrainingPlanActivityId)
            .OnDelete(DeleteBehavior.Restrict);
        builder.Entity<Report>()
            .HasOne(r => r.Specialist)
            .WithMany(u => u.ReportsCreated)
            .HasForeignKey(r => r.SpecialistId)
            .OnDelete(DeleteBehavior.Restrict);
        builder.Entity<TrainingPlanFeedback>()
            .HasOne(f => f.Child)
            .WithMany(c => c.TrainingPlanFeedbacks)
            .HasForeignKey(f => f.ChildId)
            .OnDelete(DeleteBehavior.Cascade);
        builder.Entity<TrainingPlanFeedback>()
            .HasOne(f => f.Teacher)
            .WithMany()
            .HasForeignKey(f => f.TeacherId)
            .OnDelete(DeleteBehavior.Restrict);
        builder.Entity<TrainingPlanFeedback>()
            .HasOne(f => f.TrainingPlan)
            .WithMany()
            .HasForeignKey(f => f.TrainingPlanId)
            .OnDelete(DeleteBehavior.Restrict);
        builder.Entity<Conversation>()
            .HasOne(c => c.UserA)
            .WithMany(u => u.ConversationsAsSender)
            .HasForeignKey(c => c.UserAId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.Entity<Conversation>()
            .HasOne(c => c.UserB)
            .WithMany(u => u.ConversationsAsReceiver)
            .HasForeignKey(c => c.UserBId)
            .OnDelete(DeleteBehavior.Restrict);


        builder.Entity<Message>()
            .HasOne(m => m.Conversation)
            .WithMany(c => c.Messages)
            .HasForeignKey(m => m.ConversationId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.Entity<Message>()
            .HasOne(m => m.Sender)
            .WithMany(u => u.MessagesSent)
            .HasForeignKey(m => m.SenderId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.Entity<Message>()
            .HasOne(m => m.Receiver)
            .WithMany(u => u.MessagesReceived)
            .HasForeignKey(m => m.ReceiverId)
            .OnDelete(DeleteBehavior.Restrict);


        builder.Entity<ActivityQuestion>()
    .HasOne(x => x.Activity)
    .WithMany(x => x.Questions)
    .HasForeignKey(x => x.ActivityId)
    .OnDelete(DeleteBehavior.Cascade);


        builder.Entity<QuestionOption>()
            .HasOne(x => x.Question)
            .WithMany(x => x.Options)
            .HasForeignKey(x => x.QuestionId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.Entity<Activity>()
      .HasIndex(x => x.Slug)
      .IsUnique()
      .HasFilter("[IsDeleted] = 0");


        builder.Entity<ActivityQuestion>()
    .Property(q => q.Type)
    .HasConversion<string>();


        }
    }