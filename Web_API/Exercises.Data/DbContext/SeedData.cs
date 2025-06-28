using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq.Dynamic.Core;
using System.Threading.Tasks;
using Exercises.Data.Types;
using Microsoft.AspNetCore.Identity;

namespace Exercises.Data.DbContext
{
    [SuppressMessage("ReSharper", "InconsistentNaming")]
    public class SeedData
    {
        private readonly ExercisesContext _context;
        private readonly RoleManager<Role> _roleManager;
        private readonly UserManager<User> _userManager;

        private const string USER_PASSWORD = "34er#$ER";
        
        private const string TEACHER_1 = "59c8a7a2-3a57-4754-8704-e53bfc438cb0";
        private const string TEACHER_2 = "6b8c4e62-3bcc-450b-8d2a-84ea77b2c9b9";

        private const string STUDENT_1 = "6671ffdb-fd9a-48b4-a691-98cb851b3a8f";
        private const string STUDENT_2 = "c85e3fc0-d5ac-4712-a761-18d055d81b77";

        private const string EXERCISE_COLLECTION_1 = "6fe534ae-7004-4eb7-b298-0e6452909fc7";

        private const string EXERCISE_1 = "fb21c839-741d-4fed-84d9-3439bd836c59";
        private const string EXERCISE_2 = "9abf4e82-7581-49ca-ad14-297bf2df2b55";
        private const string EXERCISE_3 = "2887df94-760d-43fb-ad65-cf21d4a803f6";
        private const string EXERCISE_4 = "7e45a3b9-3ed1-4534-aac2-d662a8afa6dc";

        private const string DIAGRAM_1 = "1572881a-88e0-443d-90d6-91f1fc6e8cec";
        private const string DIAGRAM_2 = "67f8a883-abdc-44aa-b923-2e741b6da7a7";

        public SeedData(ExercisesContext context, RoleManager<Role> roleManager, UserManager<User> userManager)
        {
            _context = context;
            _roleManager = roleManager;
            _userManager = userManager;
        }

        public async Task Init()
        {
            await CreateRoles();
            await CreateUsers();
            await CreateContent();
        }

        private async Task CreateRoles()
        {
            var roles = new List<string>
            {
                Constants.ROLE_ADMIN,
                Constants.ROLE_TEACHER,
                Constants.ROLE_STUDENT
            };

            foreach (var role in roles)
            {
                if (!await _roleManager.RoleExistsAsync(role))
                {
                    await _roleManager.CreateAsync(new Role {Name = role});
                }
            }
        }

        private async Task CreateUsers()
        {
            var teacher1 = new User
            {
                Id = new Guid(TEACHER_1),
                Email = "kosma@ceid.upatras.gr",
                UserName = "kosma@ceid.upatras.gr",
            };

            var teacher2 = new User
            {
                Id = new Guid(TEACHER_2),
                Email = "marioskosmas2@gmail.com",
                UserName = "marioskosmas2@gmail.com",
            };

            var student1 = new User
            {
                Id = new Guid(STUDENT_1),
                Email = "hello@diagrammatic.gr",
                UserName = "hello@diagrammatic.gr",
            };

            var student2 = new User
            {
                Id = new Guid(STUDENT_2),
                Email = "hello1@diagrammatic.gr",
                UserName = "hello1@diagrammatic.gr",
            };

            if (await _userManager.FindByIdAsync(TEACHER_1) == null)
            {
                await _userManager.CreateAsync(teacher1, USER_PASSWORD);
                await _userManager.AddToRoleAsync(teacher1, Constants.ROLE_TEACHER);
            }

            if (await _userManager.FindByIdAsync(TEACHER_2) == null)
            {
                await _userManager.CreateAsync(teacher2, USER_PASSWORD);
                await _userManager.AddToRoleAsync(teacher2, Constants.ROLE_TEACHER);
            }

            if (await _userManager.FindByIdAsync(STUDENT_1) == null)
            {
                await _userManager.CreateAsync(student1,USER_PASSWORD);
                await _userManager.AddToRoleAsync(student1, Constants.ROLE_STUDENT);
            }

            if (await _userManager.FindByIdAsync(STUDENT_2) == null)
            {
                await _userManager.CreateAsync(student2, USER_PASSWORD);
                await _userManager.AddToRoleAsync(student2, Constants.ROLE_STUDENT);
            }
        }


        private async Task CreateContent()
        {
            var diagram1 = new Diagram
            {
                Definition = "hi",
                Name = "diagram 1",
                Id = new Guid(DIAGRAM_1),
                Created = DateTime.Now,
                UserId = new Guid(TEACHER_1),
            };

            var diagram2 = new Diagram
            {
                Definition = "hi 2",
                Name = "diagram 2",
                Id = new Guid(DIAGRAM_2),
                Created = DateTime.Now,
                UserId = new Guid(TEACHER_1),
            };

            var exercise1 = new DiagramExercise
            {
                Id = new Guid(EXERCISE_1),
                ProblemType = ProblemType.Diagram,
                Name = "First Exercise",
                Category = "diagram",
                ExerciseCollectionId = new Guid(EXERCISE_COLLECTION_1),
                Code = "console.log('hello world');",
                Weight = 1,
                UserId = new Guid(TEACHER_1),
                Created = DateTime.Now,
                Diagram = diagram1,
                TimeToComplete = 60
            };

            var exercise2 = new PathExercise
            {
                Id = new Guid(EXERCISE_2),
                ProblemType = ProblemType.Path,
                Name = "Second Exercise",
                Category = "path",
                ExerciseCollectionId = new Guid(EXERCISE_COLLECTION_1),
                Code = "console.log('hello world 2');",
                Weight = 2,
                UserId = new Guid(TEACHER_1),
                Created = DateTime.Now,
                Diagram = diagram1,
                Paths = new[] {"1 2 5 6 9", "1 2 4 6 9", "1 7 3 8 6 9", "1 7 3 8 3 8 6 9"},
                Credits = new[] {33.33, 33.33, 33.33, 33.33},
                TimeToComplete = 120
            };

            var exercise3 = new DiagramExercise
            {
                Id = new Guid(EXERCISE_3),
                ProblemType = ProblemType.Diagram,
                Name = "Third Exercise",
                Category = "diagram",
                ExerciseCollectionId = new Guid(EXERCISE_COLLECTION_1),
                Code = "console.log('hello world');",
                Weight = 3,
                UserId = new Guid(TEACHER_1),
                Created = DateTime.Now,
                Diagram = diagram2,
                TimeToComplete = 65
            };

            var exercise4 = new PathExercise
            {
                Id = new Guid(EXERCISE_4),
                ProblemType = ProblemType.Path,
                Name = "Fourth Exercise",
                Category = "path",
                ExerciseCollectionId = new Guid(EXERCISE_COLLECTION_1),
                Code = "console.log('hello world 2');",
                Weight = 4,
                UserId = new Guid(TEACHER_1),
                Created = DateTime.Now,
                Diagram = diagram2,
                Paths = new[] {"1 2 5 6 9", "1 2 4 6 9", "1 7 3 8 6 9", "1 7 3 8 3 8 6 9"},
                Credits = new[] {33.33, 33.33, 33.33, 33.33},
                TimeToComplete = 75
            };

            var collection1 = new ExerciseCollection
            {
                Id = new Guid(EXERCISE_COLLECTION_1),
                Name = "First Exercise Set",
                Category = "new",
                Created = DateTime.Now,
                UserId = new Guid(TEACHER_1),
                Exercises = new List<Exercise>
                {
                    exercise1, exercise2, exercise3, exercise4
                }
            };

            if (_context.ExerciseCollections.Find(collection1.Id) == null)
            {
                _context.ExerciseCollections.Add(collection1);
            }

            await _context.SaveChangesAsync();
        }
    }
}