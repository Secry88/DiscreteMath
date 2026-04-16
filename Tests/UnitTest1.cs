using DiplomProject.Models;
using DiplomProject.ViewModels;
using System.Reflection;

namespace Tests
{
    public class UnitTest1
    {
        [Fact]
        public void ParseSetValues_ShouldParseCorrectly()
        {
            var method = typeof(PracticeViewModel)
                .GetMethod("ParseSetValues", BindingFlags.NonPublic | BindingFlags.Static);

            var result = method!.Invoke(null, new object[] { "1, 2, 3, 3" }) as List<int>;

            Assert.Equal(new List<int> { 1, 2, 3 }, result);
        }

        [Fact]
        public void CheckAnswer_ShouldIncreaseScore_WhenCorrect()
        {
            var vm = new PracticeViewModel();

            vm.Tasks.Add(new DiplomProject.Models.PracticeTaskDto
            {
                CorrectAnswer = "1,2,3"
            });

            vm.CurrentTask = vm.Tasks[0];
            vm.UserAnswer = "1,2,3";

            vm.CheckAnswerCommand.Execute(null);

            Assert.Equal(1, vm.Score);
        }

        [Fact]
        public void SelectRegion_ShouldSetCorrect_WhenCorrectRegion()
        {
            var vm = new EulerViewModel();

            vm.Problems.Clear();
            vm.Problems.Add(new DiplomProject.Models.EulerProblemDto
            {
                Regions = new List<DiplomProject.Models.EulerRegionDto>
        {
            new() { RegionCode = "ab", IsCorrect = true }
        }
            });

            vm.CurrentProblem = vm.Problems[0];

            vm.SelectRegionCommand.Execute("ab");

            Assert.Equal("Correct", vm.FeedbackState);
        }

        [Fact]
        public void SelectAnswer_ShouldMarkAnswerAsSelected()
        {
            var vm = new TestViewModel(1);

            var question = new DiplomProject.Models.TestQuestionDto
            {
                Id = 1,
                Answers = new List<DiplomProject.Models.TestAnswerDto>
        {
            new() { Id = 1, AnswerText = "A" },
            new() { Id = 2, AnswerText = "B" }
        }
            };

            vm.Questions.Add(question);
            vm.CurrentQuestion = question;
            var answer = question.Answers[1];
            vm.SelectAnswerCommand.Execute(answer);
            Assert.True(answer.IsSelected);
        }

        [Fact]
        public void ParseSetValues_ValidInput_ReturnsSortedUnique()
        {
            var method = typeof(PracticeViewModel)
                .GetMethod("ParseSetValues", BindingFlags.NonPublic | BindingFlags.Static);

            var result = method!.Invoke(null, new object[] { "3,1,2,2" }) as List<int>;

            Assert.Equal(new List<int> { 1, 2, 3 }, result);
        }

        [Fact]
        public void ParseSetValues_EmptyString_ReturnsEmpty()
        {
            var method = typeof(PracticeViewModel)
                .GetMethod("ParseSetValues", BindingFlags.NonPublic | BindingFlags.Static);

            var result = method!.Invoke(null, new object[] { "" }) as List<int>;

            Assert.Empty(result!);
        }

        [Fact]
        public void ParseSetValues_InvalidValues_IgnoresNonNumbers()
        {
            var method = typeof(PracticeViewModel)
                .GetMethod("ParseSetValues", BindingFlags.NonPublic | BindingFlags.Static);

            var result = method!.Invoke(null, new object[] { "1,a,2" }) as List<int>;

            Assert.Equal(new List<int> { 1, 2 }, result);
        }

        [Fact]
        public void CheckAnswer_Correct_IncreasesScore()
        {
            var vm = new PracticeViewModel();

            vm.Tasks.Add(new PracticeTaskDto { CorrectAnswer = "1,2,3" });
            vm.CurrentTask = vm.Tasks[0];
            vm.UserAnswer = "1,2,3";

            vm.CheckAnswerCommand.Execute(null);

            Assert.Equal(1, vm.Score);
            Assert.Equal("Correct", vm.FeedbackState);
        }

        [Fact]
        public void CheckAnswer_Incorrect_DoesNotIncreaseScore()
        {
            var vm = new PracticeViewModel();

            vm.Tasks.Add(new PracticeTaskDto { CorrectAnswer = "1,2,3" });
            vm.CurrentTask = vm.Tasks[0];
            vm.UserAnswer = "1,2";

            vm.CheckAnswerCommand.Execute(null);

            Assert.Equal(0, vm.Score);
            Assert.Equal("Incorrect", vm.FeedbackState);
        }

        [Fact]
        public void CheckAnswer_EmptyAnswer_NotExecuted()
        {
            var vm = new PracticeViewModel();

            vm.Tasks.Add(new PracticeTaskDto { CorrectAnswer = "1,2,3" });
            vm.CurrentTask = vm.Tasks[0];
            vm.UserAnswer = "";

            vm.CheckAnswerCommand.Execute(null);

            Assert.Equal(0, vm.Score);
        }

        [Fact]
        public void NextTask_ChangesTask()
        {
            var vm = new PracticeViewModel();

            vm.Tasks.Add(new PracticeTaskDto());
            vm.Tasks.Add(new PracticeTaskDto());

            vm.CurrentTask = vm.Tasks[0];
            vm.NextTaskCommand.Execute(null);

            Assert.Equal(vm.Tasks[1], vm.CurrentTask);
        }

        [Fact]
        public void SelectRegion_Correct_SetsCorrectState()
        {
            var vm = new EulerViewModel();

            vm.Problems.Clear();
            vm.Problems.Add(new EulerProblemDto
            {
                Regions = new List<EulerRegionDto>
        {
            new() { RegionCode = "ab", IsCorrect = true }
        }
            });

            vm.CurrentProblem = vm.Problems[0];

            vm.SelectRegionCommand.Execute("ab");

            Assert.Equal("Correct", vm.FeedbackState);
        }

        [Fact]
        public void SelectRegion_Wrong_SetsIncorrectState()
        {
            var vm = new EulerViewModel();

            vm.Problems.Clear();
            vm.Problems.Add(new EulerProblemDto
            {
                Regions = new List<EulerRegionDto>
        {
            new() { RegionCode = "ab", IsCorrect = true }
        }
            });

            vm.CurrentProblem = vm.Problems[0];

            vm.SelectRegionCommand.Execute("a-only");

            Assert.Equal("Incorrect", vm.FeedbackState);
        }

        [Fact]
        public void SelectRegion_NullProblem_DoesNothing()
        {
            var vm = new EulerViewModel();

            vm.CurrentProblem = null;

            vm.SelectRegionCommand.Execute("ab");

            Assert.Equal("None", vm.FeedbackState);
        }

        [Fact]
        public void GoNext_LastProblem_FinishesSession()
        {
            var vm = new EulerViewModel();

            vm.Problems.Clear();
            vm.Problems.Add(new EulerProblemDto());
            vm.CurrentProblem = vm.Problems[0];
            vm.CurrentIndex = 0;

            vm.NextCommand.Execute(null);

            Assert.True(vm.IsSessionFinished);
        }

        [Fact]
        public void SelectAnswer_SetsOnlyOneSelected()
        {
            var vm = new TestViewModel(1);

            var q = new TestQuestionDto
            {
                Id = 1,
                Answers = new List<TestAnswerDto>
        {
            new() { Id = 1 },
            new() { Id = 2 }
        }
            };

            vm.Questions.Add(q);
            vm.CurrentQuestion = q;

            vm.SelectAnswerCommand.Execute(q.Answers[0]);

            Assert.True(q.Answers[0].IsSelected);
            Assert.False(q.Answers[1].IsSelected);
        }

        [Fact]
        public void NextQuestion_IncreasesIndex()
        {
            var vm = new TestViewModel(1);

            var q1 = new TestQuestionDto { Id = 1, Answers = new List<TestAnswerDto>() };
            var q2 = new TestQuestionDto { Id = 2, Answers = new List<TestAnswerDto>() };

            vm.Questions.Add(q1);
            vm.Questions.Add(q2);

            vm.CurrentQuestion = q1;

            vm.NextQuestionCommand.Execute(null);

            Assert.Equal(q2, vm.CurrentQuestion);
        }

        [Fact]
        public void PreviousQuestion_OnFirst_DoesNotChange()
        {
            var vm = new TestViewModel(1);

            var q1 = new TestQuestionDto { Id = 1, Answers = new List<TestAnswerDto>() };

            vm.Questions.Add(q1);
            vm.CurrentQuestion = q1;

            vm.PreviousQuestionCommand.Execute(null);

            Assert.Equal(q1, vm.CurrentQuestion);
        }


    }
}
