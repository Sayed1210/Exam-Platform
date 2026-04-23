using ExamApi.Models;

using Microsoft.EntityFrameworkCore;

    public class ApiContext : DbContext
    {
        public ApiContext(DbContextOptions<ApiContext> options) : base(options) { }

        public DbSet<User> User { get; set; }
        public DbSet<Candidate> Candidates { get; set; }
        public DbSet<Topic> Topics { get; set; }
        public DbSet<Question> Questions { get; set; }
        public DbSet<Choice> Choices { get; set; }
        public DbSet<Exam> Exams { get; set; }
        public DbSet<ExamQuestion> ExamQuestions { get; set; }
        public DbSet<CandidateAnswer> CandidateAnswers { get; set; }
        public DbSet<CandidateExam> CandidateExams { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Composite keys for junction tables
            modelBuilder.Entity<ExamQuestion>()
                .HasKey(eq => new { eq.ExamId, eq.QuestionId });

            modelBuilder.Entity<CandidateAnswer>()
                .HasKey(ca => new { ca.CandidateId, ca.QuestionId, ca.ExamId });

            modelBuilder.Entity<CandidateExam>()
                .HasKey(ce => new { ce.CandidateId, ce.ExamId });

            modelBuilder.Entity<CandidateExam>()
                .Property(ce => ce.Status)
                .HasConversion<string>();
            
            modelBuilder.Entity<User>()
                .Property(ce => ce.Role)
                .HasConversion<string>();
    }
}
