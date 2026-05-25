namespace Exam.Repo;
using Exam.Models;

public interface ICandidateAnswerRepository
{
    // inserting multiple CandidateAnswer records in one operation
    // More efficient than calling Add() in a loop because EF tracks all entities and saves them together
    Task AddRangeAsync(List<CandidateAnswer> answers);
    Task<CandidateAnswer?> GetAsync(int candidateId, int examId, int questionId);
    Task<List<CandidateAnswer>> GetByCandidateExamAsync(int candidateId, int examId);
    Task AddOrUpdateAsync(CandidateAnswer answer);
    Task<bool> ChoiceBelongsToQuestionAsync(int choiceId, int questionId);
    Task<List<int>> GetCorrectChoiceIdsAsync(List<int> questionIds);
}
