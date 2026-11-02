namespace OnlineLearningPlatform.Models;
public class Enrollment { public int StudentId { get; set; } public int CourseId { get; set; } public DateTime EnrolledAt { get; set; } public decimal Progress { get; set; } public EnrollmentStatus Status { get; set; } public decimal? FinalGrade { get; set; } }
