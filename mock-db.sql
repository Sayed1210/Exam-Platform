USE examdb;

-- =========================
-- DISABLE FOREIGN KEYS
-- =========================

SET FOREIGN_KEY_CHECKS = 0;

-- =========================
-- CLEAR TABLES
-- =========================

TRUNCATE TABLE ExamQuestions;
TRUNCATE TABLE Choices;
TRUNCATE TABLE Questions;
TRUNCATE TABLE Exams;
TRUNCATE TABLE Topics;

SET FOREIGN_KEY_CHECKS = 1;

-- =========================
-- INSERT TOPICS
-- =========================

INSERT INTO Topics (Title) VALUES
('OOP'),
('Database'),
('C#'),
('.NET'),
('Git'),
('Web Development'),
('Algorithms');

-- =========================
-- INSERT QUESTIONS
-- =========================

INSERT INTO Questions (TopicId, Text, ImageUrl) VALUES

-- OOP
(1, 'What does OOP stand for?', NULL),
(1, 'Which principle hides implementation details?', NULL),
(1, 'What is inheritance?', NULL),
(1, 'Which keyword is used for inheritance in C#?', NULL),
(1, 'What is polymorphism?', NULL),

-- Database
(2, 'What does SQL stand for?', NULL),
(2, 'Which SQL command retrieves data?', NULL),
(2, 'What is a primary key?', NULL),
(2, 'Which JOIN returns matching rows only?', NULL),
(2, 'What does CRUD stand for?', NULL),

-- C#
(3, 'Which symbol is used for comments in C#?', NULL),
(3, 'Which type stores true/false values?', NULL),
(3, 'What is the entry point of a C# application?', NULL),
(3, 'Which keyword creates an object?', NULL),
(3, 'Which collection stores key-value pairs?', NULL),

-- .NET
(4, 'What is ASP.NET Core?', NULL),
(4, 'Which file contains app settings in .NET?', NULL),
(4, 'What does EF Core stand for?', NULL),
(4, 'Which command creates a migration?', NULL),
(4, 'What is dependency injection?', NULL),

-- Git
(5, 'Which command uploads commits to GitHub?', NULL),
(5, 'Which command creates a new branch?', NULL),
(5, 'What does git pull do?', NULL),
(5, 'Which command checks repository status?', NULL),

-- Web Development
(6, 'What does HTML stand for?', NULL),
(6, 'Which CSS property changes text color?', NULL),
(6, 'Which JavaScript keyword declares a variable?', NULL),
(6, 'What does API stand for?', NULL),

-- Algorithms
(7, 'What is the time complexity of binary search?', NULL),
(7, 'Which data structure uses FIFO?', NULL);

-- =========================
-- INSERT CHOICES
-- =========================

INSERT INTO Choices (QuestionId, Text, IsCorrect, ImageUrl) VALUES

-- Q1
(1, 'Object Oriented Programming', true, NULL),
(1, 'Object Ordered Programming', false, NULL),
(1, 'Operation Object Programming', false, NULL),
(1, 'Only Object Programming', false, NULL),

-- Q2
(2, 'Encapsulation', true, NULL),
(2, 'Inheritance', false, NULL),
(2, 'Polymorphism', false, NULL),
(2, 'Compilation', false, NULL),

-- Q3
(3, 'Reusing code from another class', true, NULL),
(3, 'Deleting objects', false, NULL),
(3, 'Hiding methods', false, NULL),
(3, 'Database connection', false, NULL),

-- Q4
(4, ':', true, NULL),
(4, 'extends', false, NULL),
(4, 'inherits', false, NULL),
(4, '->', false, NULL),

-- Q5
(5, 'One interface many forms', true, NULL),
(5, 'Database relation', false, NULL),
(5, 'Code duplication', false, NULL),
(5, 'A loop type', false, NULL),

-- Q6
(6, 'Structured Query Language', true, NULL),
(6, 'Simple Query Language', false, NULL),
(6, 'System Query Language', false, NULL),
(6, 'Sequential Query Language', false, NULL),

-- Q7
(7, 'SELECT', true, NULL),
(7, 'GET', false, NULL),
(7, 'FETCH', false, NULL),
(7, 'SHOW', false, NULL),

-- Q8
(8, 'A unique identifier for a row', true, NULL),
(8, 'A duplicated column', false, NULL),
(8, 'A foreign table', false, NULL),
(8, 'An index only', false, NULL),

-- Q9
(9, 'INNER JOIN', true, NULL),
(9, 'LEFT JOIN', false, NULL),
(9, 'RIGHT JOIN', false, NULL),
(9, 'FULL JOIN', false, NULL),

-- Q10
(10, 'Create Read Update Delete', true, NULL),
(10, 'Copy Run Update Delete', false, NULL),
(10, 'Create Remove Upload Download', false, NULL),
(10, 'Control Read Use Delete', false, NULL),

-- Q11
(11, '//', true, NULL),
(11, '#', false, NULL),
(11, '<!-- -->', false, NULL),
(11, '**', false, NULL),

-- Q12
(12, 'bool', true, NULL),
(12, 'string', false, NULL),
(12, 'int', false, NULL),
(12, 'double', false, NULL),

-- Q13
(13, 'Main()', true, NULL),
(13, 'Start()', false, NULL),
(13, 'Run()', false, NULL),
(13, 'Init()', false, NULL),

-- Q14
(14, 'new', true, NULL),
(14, 'create', false, NULL),
(14, 'make', false, NULL),
(14, 'object', false, NULL),

-- Q15
(15, 'Dictionary', true, NULL),
(15, 'List', false, NULL),
(15, 'Array', false, NULL),
(15, 'Queue', false, NULL),

-- Q16
(16, 'A web framework for .NET', true, NULL),
(16, 'A database', false, NULL),
(16, 'An operating system', false, NULL),
(16, 'A browser', false, NULL),

-- Q17
(17, 'appsettings.json', true, NULL),
(17, 'config.xml', false, NULL),
(17, 'settings.ini', false, NULL),
(17, 'program.cs', false, NULL),

-- Q18
(18, 'Entity Framework Core', true, NULL),
(18, 'Extended Framework Core', false, NULL),
(18, 'Entity Function Core', false, NULL),
(18, 'Embedded Framework Core', false, NULL),

-- Q19
(19, 'dotnet ef migrations add', true, NULL),
(19, 'dotnet add migration', false, NULL),
(19, 'ef create migration', false, NULL),
(19, 'migration add', false, NULL),

-- Q20
(20, 'Providing dependencies automatically', true, NULL),
(20, 'Deleting services', false, NULL),
(20, 'Compiling code', false, NULL),
(20, 'Managing databases', false, NULL),

-- Q21
(21, 'git push', true, NULL),
(21, 'git upload', false, NULL),
(21, 'git send', false, NULL),
(21, 'git commit', false, NULL),

-- Q22
(22, 'git branch', true, NULL),
(22, 'git checkout', false, NULL),
(22, 'git init', false, NULL),
(22, 'git merge', false, NULL),

-- Q23
(23, 'Fetches and merges changes', true, NULL),
(23, 'Deletes repository', false, NULL),
(23, 'Creates branch', false, NULL),
(23, 'Uploads code', false, NULL),

-- Q24
(24, 'git status', true, NULL),
(24, 'git check', false, NULL),
(24, 'git state', false, NULL),
(24, 'git info', false, NULL),

-- Q25
(25, 'HyperText Markup Language', true, NULL),
(25, 'HighText Machine Language', false, NULL),
(25, 'Hyper Transfer Markup Language', false, NULL),
(25, 'Home Tool Markup Language', false, NULL),

-- Q26
(26, 'color', true, NULL),
(26, 'background-color', false, NULL),
(26, 'font-size', false, NULL),
(26, 'text-style', false, NULL),

-- Q27
(27, 'let', true, NULL),
(27, 'define', false, NULL),
(27, 'int', false, NULL),
(27, 'string', false, NULL),

-- Q28
(28, 'Application Programming Interface', true, NULL),
(28, 'Application Process Internet', false, NULL),
(28, 'Applied Programming Interface', false, NULL),
(28, 'Advanced Programming Internet', false, NULL),

-- Q29
(29, 'O(log n)', true, NULL),
(29, 'O(n)', false, NULL),
(29, 'O(n^2)', false, NULL),
(29, 'O(1)', false, NULL),

-- Q30
(30, 'Queue', true, NULL),
(30, 'Stack', false, NULL),
(30, 'Tree', false, NULL),
(30, 'Graph', false, NULL);

-- =========================
-- INSERT EXAMS
-- =========================

INSERT INTO Exams (Title, DurationMins, CreatedAt) VALUES
('C# Fundamentals Exam', 60, NOW()),
('Database Basics Exam', 45, NOW()),
('Full Software Exam', 90, NOW());

-- =========================
-- INSERT EXAM QUESTIONS
-- =========================

INSERT INTO ExamQuestions (ExamId, QuestionId) VALUES

-- Exam 1
(1, 11),
(1, 12),
(1, 13),
(1, 14),
(1, 15),
(1, 16),
(1, 17),
(1, 18),

-- Exam 2
(2, 6),
(2, 7),
(2, 8),
(2, 9),
(2, 10),

-- Exam 3
(3, 1),
(3, 2),
(3, 3),
(3, 4),
(3, 5),
(3, 6),
(3, 7),
(3, 8),
(3, 9),
(3, 10),
(3, 11),
(3, 12),
(3, 13),
(3, 14),
(3, 15),
(3, 16),
(3, 17),
(3, 18),
(3, 19),
(3, 20),
(3, 21),
(3, 22),
(3, 23),
(3, 24),
(3, 25),
(3, 26),
(3, 27),
(3, 28),
(3, 29),
(3, 30);

-- =========================
-- VERIFY
-- =========================

SELECT COUNT(*) AS TopicsCount FROM Topics;
SELECT COUNT(*) AS QuestionsCount FROM Questions;
SELECT COUNT(*) AS ChoicesCount FROM Choices;
SELECT COUNT(*) AS ExamsCount FROM Exams;
SELECT COUNT(*) AS ExamQuestionsCount FROM ExamQuestions;

INSERT INTO Candidates (Email, FirstName, LastName, Phone) VALUES
('ahmed.hassan@example.com', 'Ahmed', 'Hassan', '01010000001'),
('mohamed.ali@example.com', 'Mohamed', 'Ali', '01010000002'),
('sara.ibrahim@example.com', 'Sara', 'Ibrahim', '01010000003'),
('nour.khaled@example.com', 'Nour', 'Khaled', '01010000004'),
('youssef.adel@example.com', 'Youssef', 'Adel', '01010000005'),
('mariam.tarek@example.com', 'Mariam', 'Tarek', '01010000006'),
('omar.sameh@example.com', 'Omar', 'Sameh', '01010000007'),
('fatma.essam@example.com', 'Fatma', 'Essam', '01010000008'),
('karim.nabil@example.com', 'Karim', 'Nabil', '01010000009');