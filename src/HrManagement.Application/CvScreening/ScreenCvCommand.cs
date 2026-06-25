using HrManagement.Application.Abstractions.Ai;
using MediatR;

namespace HrManagement.Application.CvScreening;

public sealed record ScreenCvCommand(string CvText, string JobDescription) : IRequest<CvScreeningResult>;
