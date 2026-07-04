# HR Admin User Onboarding Guide

This guide is written for HR administrators who will use the HR Management API. It assumes no software development background.

## 1. Getting Access

1. Ask your IT administrator for a username and password.
2. You will receive a token that looks like a long string of letters and numbers.
3. Store this token securely. You will need it for every API call.

## 2. First Steps After Login

### Create a Department

Before adding employees, create at least one department:

```bash
curl -X POST https://your-api/api/departments \
  -H "Authorization: Bearer YOUR_TOKEN" \
  -H "Content-Type: application/json" \
  -d '{"name": "Engineering"}'
```

### Create a Role

Create a role that matches your company's job titles:

```bash
curl -X POST https://your-api/api/roles \
  -H "Authorization: Bearer YOUR_TOKEN" \
  -H "Content-Type: application/json" \
  -d '{"title": "Senior Engineer"}'
```

### Add an Employee

Use the department and role IDs from the previous steps:

```bash
curl -X POST https://your-api/api/employees \
  -H "Authorization: Bearer YOUR_TOKEN" \
  -H "Content-Type: application/json" \
  -d '{
    "firstName": "Ada",
    "lastName": "Lovelace",
    "email": "ada@example.com",
    "departmentId": "UUID_FROM_DEPARTMENT",
    "roleId": "UUID_FROM_ROLE"
  }'
```

## 3. Managing Leave Requests

Employees submit leave requests through the system. As an HR admin, you can:

### View All Leave Requests

```bash
curl -X GET "https://your-api/api/leave-requests?page=1&pageSize=50" \
  -H "Authorization: Bearer YOUR_TOKEN"
```

### Filter Leave Requests

Filter by employee, date range, or status:

```bash
curl -X GET "https://your-api/api/leave-requests?status=Approved&startDate=2026-01-01" \
  -H "Authorization: Bearer YOUR_TOKEN"
```

### Approve or Reject

```bash
curl -X POST https://your-api/api/leave-requests/REQUEST_ID/approve \
  -H "Authorization: Bearer YOUR_TOKEN" \
  -H "Content-Type: application/json" \
  -d '{"reviewerEmployeeId": "YOUR_EMPLOYEE_ID", "notes": "Enjoy your vacation!"}'
```

## 4. Using AI Features

The system includes two AI-powered endpoints that require a running local LLM provider (Ollama or LM Studio).

### CV Screening

Send a candidate's CV text and a job description to get a structured match assessment:

```bash
curl -X POST https://your-api/api/ai/cv-screening \
  -H "Authorization: Bearer YOUR_TOKEN" \
  -H "Content-Type: application/json" \
  -d '{
    "cvText": "Full candidate resume text here...",
    "jobDescription": "Job description text here..."
  }'
```

### Performance Review Analysis

Send a peer review text to get sentiment and key points:

```bash
curl -X POST https://your-api/api/ai/performance-review-analysis \
  -H "Authorization: Bearer YOUR_TOKEN" \
  -H "Content-Type: application/json" \
  -d '{"reviewText": "Peer review text here..."}'
```

**Note:** These endpoints will return an error if the LLM provider is not running or not reachable.

## 5. Common Issues

| Problem | Solution |
|---------|----------|
| 401 Unauthorized | Your token has expired or is invalid. Contact your IT administrator. |
| 403 Forbidden | You do not have the required HR or Admin role. |
| 400 Bad Request | Check your request body format. Required fields must not be empty. |
| 503 Service Unavailable | The database or LLM provider is temporarily unreachable. Try again later. |

## 6. Support

For technical issues, contact your system administrator. For questions about leave policies or employee data, contact your HR director.
