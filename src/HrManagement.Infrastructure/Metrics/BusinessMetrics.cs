using Prometheus;

namespace HrManagement.Infrastructure.Metrics;

public static class BusinessMetrics
{
    public static readonly Counter LeaveRequestsCreated = Prometheus.Metrics
        .CreateCounter("hr_leave_requests_created_total", "Total number of leave requests created.", new CounterConfiguration
        {
            LabelNames = new[] { "employee_id" }
        });

    public static readonly Counter LeaveRequestsApproved = Prometheus.Metrics
        .CreateCounter("hr_leave_requests_approved_total", "Total number of leave requests approved.", new CounterConfiguration
        {
            LabelNames = new[] { "reviewer_id" }
        });

    public static readonly Counter LeaveRequestsRejected = Prometheus.Metrics
        .CreateCounter("hr_leave_requests_rejected_total", "Total number of leave requests rejected.", new CounterConfiguration
        {
            LabelNames = new[] { "reviewer_id" }
        });

    public static readonly Counter EmployeesCreated = Prometheus.Metrics
        .CreateCounter("hr_employees_created_total", "Total number of employee records created.");

    public static readonly Counter EmployeesUpdated = Prometheus.Metrics
        .CreateCounter("hr_employees_updated_total", "Total number of employee records updated.");

    public static readonly Counter EmployeesDeleted = Prometheus.Metrics
        .CreateCounter("hr_employees_deleted_total", "Total number of employee records deleted.");

    public static readonly Counter AiCvScreeningCompleted = Prometheus.Metrics
        .CreateCounter("hr_ai_cv_screening_completed_total", "Total number of CV screening completions.", new CounterConfiguration
        {
            LabelNames = new[] { "status" }
        });

    public static readonly Counter AiPerformanceReviewAnalyzed = Prometheus.Metrics
        .CreateCounter("hr_ai_performance_review_analyzed_total", "Total number of performance review analyses.", new CounterConfiguration
        {
            LabelNames = new[] { "status" }
        });

    public static readonly Counter AuthLogins = Prometheus.Metrics
        .CreateCounter("hr_auth_logins_total", "Total number of successful login attempts.", new CounterConfiguration
        {
            LabelNames = new[] { "username" }
        });

    public static readonly Counter AuthLoginFailures = Prometheus.Metrics
        .CreateCounter("hr_auth_login_failures_total", "Total number of failed login attempts.");
}
