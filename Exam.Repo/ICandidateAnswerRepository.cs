namespace Exam.Repositories;
using Exam.Models;

public interface ICandidateAnswerRepository
{
    // inserting multiple CandidateAnswer records in one operation
    // More efficient than calling Add() in a loop because EF tracks all entities and saves them together
    Task AddRangeAsync(List<CandidateAnswer> answers);
    Task<List<int>> GetCorrectChoiceIdsAsync(int questionId);

}
