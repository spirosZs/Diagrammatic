using System.Linq;
using AutoMapper;
using Exercises.Common.Diagram;
using Exercises.Common.Exam;
using Exercises.Common.Exercise;
using Exercises.Common.ExerciseCollection;
using Exercises.Common.Submission;
using Exercises.Data;
using Exercises.Data.DiagramDefinitions;
using System.Text.Json;
using Exercises.Common.User;

namespace Exercises.Helpers
{
    public static class AutoMapper
    {
        public static void Init(this IMapperConfigurationExpression cfg)
        {
            //USER
            cfg
                .CreateMap<User, UserDtoBase>();

            //EXERCISE
            cfg
                .CreateMap<Exercise, ExerciseDto>()
                .Include<DiagramExercise, DiagramExerciseDto>()
                .Include<PathExercise, PathExerciseDto>();

            //EXERCISE GAME
            cfg
                .CreateMap<Exercise, ExerciseGameDto>()
                .Include<DiagramExercise, DiagramExerciseGameDto>()
                .Include<PathExercise, PathExerciseGameDto>();
            
            //EXERCISE TYPE DIAGRAM 
            cfg.CreateMap<DiagramExercise, DiagramExerciseDto>();
            cfg.CreateMap<DiagramExercise, DiagramExerciseGameDto>();
            
            //EXERCISE TYPE PATH
            cfg.CreateMap<PathExercise, PathExerciseDto>()
                .ForMember(
                    dest => dest.Paths, opt => opt.MapFrom(src => src.Paths.Select((p, index) =>
                        new PathDto
                        {
                            Path = p,
                            Credits = src.Credits[index]
                        })
                    ));
            cfg.CreateMap<PathExercise, PathExerciseGameDto>();

            //EXERCISE DTO -> EXERCISE
            cfg
                .CreateMap<ExerciseDto, Exercise>()
                .Include<DiagramExerciseDto, DiagramExercise>()
                .Include<PathExerciseDto, PathExercise>();
            cfg.CreateMap<DiagramExerciseDto, DiagramExercise>();
            cfg.CreateMap<PathExerciseDto, PathExercise>();
            
            //EXERCISE CREATE DTO -> EXERCISE
            cfg
                .CreateMap<ExerciseCreateDto, Exercise>()
                .Include<DiagramExerciseCreateDto, DiagramExercise>()
                .Include<PathExerciseCreateDto, PathExercise>();
            cfg.CreateMap<DiagramExerciseCreateDto, DiagramExercise>();
            cfg.CreateMap<PathExerciseCreateDto, PathExercise>()
                .ForMember(dest => dest.Paths, opt => opt.MapFrom(src => src.Paths.Select(p => p.Path)))
                .ForMember(dest => dest.Credits, opt => opt.MapFrom(src => src.Paths.Select(p => p.Credits)));

            //UPDATE EXERCISE DTO -> EXERCISE 
            cfg
                .CreateMap<ExerciseUpdateDto, Exercise>()
                .Include<DiagramExerciseUpdateDto, DiagramExercise>()
                .Include<PathExerciseUpdateDto, PathExercise>();
            cfg.CreateMap<DiagramExerciseUpdateDto, DiagramExercise>();
            cfg.CreateMap<PathExerciseUpdateDto, PathExercise>()
                .ForMember(dest => dest.Paths, opt => opt.MapFrom(src => src.Paths.Select(p => p.Path)))
                .ForMember(dest => dest.Credits, opt => opt.MapFrom(src => src.Paths.Select(p => p.Credits)));
//            cfg.ValidateInlineMaps = false;
            
            //EXERCISE COLLECTION
            cfg.CreateMap<ExerciseCollectionCreateDto, ExerciseCollection>();

            cfg.CreateMap<ExerciseCollection, ExerciseCollectionDto>();
            cfg.CreateMap<ExerciseCollection, ExerciseCollectionWithExercisesDto>();

            //EXAM
            cfg.CreateMap<ExamCreateDto, Exam>();
            cfg
                .CreateMap<Exam, ExamDto>()
                .ForPath(
                    dest => dest.Participants.Students, opts => 
                    opts.MapFrom(src => src.Participations.Select(x => x.User).ToList())
                    );
            cfg.CreateMap<Exam, ExamWithExercisesDto>();

            //SUBMISSION
            cfg.CreateMap<SubmissionCreateDto, Submission>()
                .ForMember(d => d.Data, opt =>
                    opt.MapFrom(s =>
                        Newtonsoft.Json.JsonConvert.SerializeObject(s.Data)
                        )
                );

            cfg.CreateMap<Submission, SubmissionDto>();

            //DIAGRAM
            cfg.CreateMap<DiagramCreateDto, Diagram>();
            cfg.CreateMap<DiagramUpdateDto, Diagram>();
            cfg.CreateMap<Diagram, DiagramDto>();

            //DIAGRAM DEFINITION 
            cfg.CreateMap<DiagramDefinitionDto, DiagramDefinition>();
            cfg.CreateMap<NodeDefinitionDto, NodeDefinition>()
                .ForMember(dest => dest.Number,
                    opt => opt.MapFrom(
                        src => src.Annotations.FirstOrDefault().Content
                    )
                );
            cfg.CreateMap<ConnectorDefinitionDto, ConnectorDefinition>();
        }
    }
}