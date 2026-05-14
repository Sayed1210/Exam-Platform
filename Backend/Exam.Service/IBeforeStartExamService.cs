public interface IBeforeStartExamService
{
    Task<(BeforeStartExamResponse? Response, string? Error)>
        GetExamInfo(string token);
}