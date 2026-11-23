using System;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using Exercises.Common;
using Exercises.Common.Abstractions;
using Exercises.Common.Exam;
using Exercises.Common.Exam.Game;
using Exercises.Common.Exercise;
using Exercises.Core.Helpers;
using Exercises.Data;
using Exercises.Data.Types;
using Exercises.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Exercises.Controllers
{
    /// <summary>
    /// A Game is basically an Exam.
    /// Documented bellow are methods about a game (start/participation etc) and not about game creation.
    /// </summary>
    [Route(Router.Game)]
    [Authorize]
    public class GameController : Controller
    {
        private readonly IIdentityService _identityService;
        private readonly IExamService _examService;

        public GameController(IServiceProvider serviceProvider, IIdentityService identityService)
        {
            _identityService = identityService;
            _examService =
                (IExamService) serviceProvider.GetService(typeof(IExamService));
        }

        /// <summary>
        /// Participate in a game.
        /// </summary>
        /// <param name="examParticipationDto"></param>
        /// <param name="token"></param>
        /// <response code="200">Successful response. Returns the id of the exam that this code refers to.</response>
        /// <response code="404">Not found.</response>  
        /// <response code="400">Bad Request.</response>  
        [HttpPost("participate")]
        [Produces("application/json")]
        [ProducesResponseType(200, Type = typeof(GenericGameDefinitionDto))]
        [ProducesResponseType(404)]
        [ProducesResponseType(400)]
        public async Task<IActionResult> Participate([FromBody] ExamParticipationDto examParticipationDto,
            CancellationToken token = default)
        {
            var inputCode = examParticipationDto.ParticipationCode;
            var exam = await _examService.GetAsync(inputCode, token);
            if (exam == null)
            {
                return NotFound();
            }

            if (inputCode != exam.ParticipationCode)
            {
                ModelState.AddModelError(nameof(exam.ParticipationCode), $"Code {inputCode} is not valid.");
                return BadRequest(ModelState);
            }

            await _examService.Participate(exam, HttpContext.GetUserId(), token);
            return Ok(new
            {
                Id = exam.Id,
                Name = exam.Name
            });
        }

        /// <summary>
        /// Get Game info.
        /// </summary>
        /// <param name="id"></param>
        /// <param name="token"></param>
        /// <response code="200">Successful response.</response>
        /// <response code="404">Not found.</response>  
        /// <response code="400">Bad Request.</response>  
        [HttpGet("{id}")]
        [Produces("application/json")]
        [ProducesResponseType(200, Type = typeof(GameInfoDto))]
        [ProducesResponseType(404)]
        [ProducesResponseType(400)]
        [AllowAnonymous]
        public async Task<IActionResult> Game([FromRoute] Guid id, CancellationToken token = default)
        {
            // Settle any elapsed round deadline first, so the reported state is never
            // stale just because the worker missed its cue.
            var exam = await _examService.SyncProgress(id, token);
            var response = exam.MapToGameInfoDto();
            return Ok(response);
        }

        /// <summary>
        /// Get Current Round Exercise.
        /// </summary>
        /// <param name="id"></param>
        /// <param name="token"></param>
        /// <response code="200">Successful response.</response>
        /// <response code="404">Not found.</response>  
        /// <response code="400">Bad Request.</response>  
        [HttpGet("{id}/exercise")]
        [Produces("application/json")]
        [ProducesResponseType(200, Type = typeof(ExerciseDto))]
        [ProducesResponseType(404)]
        [ProducesResponseType(400)]
        public async Task<IActionResult> GetRoundExercise([FromRoute] Guid id, CancellationToken token = default)
        {
            try
            {
                // Same catch-up as GET /api/game/{id}: a client asking which exercise is
                // current must not be handed a round whose time is already up.
                var exam = await _examService.SyncProgress(id, token);
                var exercise = exam.CurrentExercise;

                if (exercise == null)
                {
                    return NoContent();
                }

                var mappedExercise = Mapper.Map<ExerciseDto>(exercise);
                WithholdSolutionFromStudents(mappedExercise);
                return Ok(mappedExercise);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message, type = ex.GetType().Name });
            }
        }

        /// <summary>
        /// Blanks the answer a student is about to be asked for.
        /// </summary>
        /// <remarks>
        /// A diagram exercise keeps the teacher's solution graph in Diagram.Definition, and
        /// the round's exercise is sent to everyone playing — so the answer was sitting in
        /// the student's own network tab while they were being timed on drawing it. Nothing
        /// on their screen uses the field (the drawing they are compared against is never
        /// shown to them), so it is dropped on the way out. Scoring is unaffected: it
        /// happens server-side against the stored diagram, not against anything the client
        /// sends back.
        ///
        /// A path exercise is left alone — there Definition is the question text the class
        /// reads, and its answer lives in Paths, which this DTO never carries.
        /// </remarks>
        private void WithholdSolutionFromStudents(ExerciseDto exercise)
        {
            if (exercise?.Diagram == null) return;
            if (User.IsInRole(Constants.ROLE_TEACHER) || User.IsInRole(Constants.ROLE_ADMIN)) return;
            if (!string.Equals(exercise.ProblemType, ProblemType.Diagram.ToString(),
                    StringComparison.OrdinalIgnoreCase)) return;

            exercise.Diagram.Definition = string.Empty;
        }

        /// <summary>
        /// Start a game.
        /// </summary>
        /// <param name="id"></param>
        /// <param name="token"></param>
        /// <response code="200">Successful response.</response>
        /// <response code="404">Not found.</response>  
        /// <response code="400">Bad Request.</response>  
        [HttpPost("{id}/start")]
        [Produces("application/json")]
        [ProducesResponseType(200, Type = typeof(GameInfoDto))]
        [ProducesResponseType(404)]
        [ProducesResponseType(400)]
        public async Task<IActionResult> StartGame([FromRoute] Guid id, CancellationToken token = default)
        {
            var exam = await _examService.StartGame(id, token);
            if (exam.DateStarted == null)
            {
                throw new Exception("Could not start the game. Try again later.");
            }

            var response = exam.MapToGameInfoDto();
            return Ok(response);
        }

        /// <summary>
        /// End game.
        /// </summary>
        /// <param name="id"></param>
        /// <param name="token"></param>
        /// <response code="200">Successful response.</response>
        /// <response code="404">Not found.</response>  
        /// <response code="400">Bad Request.</response>  
        [HttpPost("{id}/end")]
        [Produces("application/json")]
        [ProducesResponseType(200, Type = typeof(GameInfoDto))]
        [ProducesResponseType(404)]
        [ProducesResponseType(400)]
        public async Task<IActionResult> EndGame([FromRoute] Guid id, CancellationToken token = default)
        {
            var exam = await _examService.EndGame(id, token);
            var response = exam.MapToGameInfoDto();
            return Ok(response);
        }

        /// <summary>
        /// Update game definitions.
        /// </summary>
        /// <param name="id"></param>
        /// <param name="body"></param>
        /// <param name="token"></param>
        /// <response code="200">Successful response.</response>
        /// <response code="404">Not found.</response>  
        /// <response code="400">Bad Request.</response>  
        [HttpPost("{id}/update")]
        [Produces("application/json")]
        [ProducesResponseType(200, Type = typeof(GameInfoDto))]
        [ProducesResponseType(404)]
        [ProducesResponseType(400)]
        public async Task<IActionResult> UpdateGame(
            [FromRoute] Guid id,
            [FromBody] UpdateGameDto body,
            CancellationToken token = default)
        {
            var exam = await _examService.UpdateGame(id, body, token);
            var response = exam.MapToGameInfoDto();
            return Ok(response);
        }

        /// <summary>
        /// Update the current exercise in an ongoing game.
        /// </summary>
        /// <param name="id"></param>
        /// <param name="body"></param>
        /// <param name="token"></param>
        /// <response code="200">Successful response.</response>
        [HttpPost("{id}/update-exercise")]
        [Produces("application/json")]
        [ProducesResponseType(200, Type = typeof(GameInfoDto))]
        [ProducesResponseType(404)]
        [ProducesResponseType(400)]
        public async Task<IActionResult> UpdateExercise(
            [FromRoute] Guid id,
            [FromBody] UpdateGameExerciseDto body,
            CancellationToken token = default
        )
        {
            var exam = await _examService.UpdateExercise(id, body, token);
            var response = exam.MapToGameInfoDto();
            return Ok(response);
        }
    }
}