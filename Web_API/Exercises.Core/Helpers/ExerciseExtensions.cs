using System;
using System.Collections.Generic;
using Exercises.Data;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Exercises.Common;
using Exercises.Common.Diagram;
using Exercises.Data.Abstractions;
using Exercises.Data.DbContext;
using Exercises.Data.DiagramDefinitions;
using Microsoft.EntityFrameworkCore;

namespace Exercises.Core.Helpers
{
    public static class ExerciseExtensions
    {
        public static T DetachEntity<T>(this ExercisesContext db, T entity) where T : class
        {
            db.Entry(entity).State = EntityState.Detached;
            entity.GetType().GetProperty("Id")?.SetValue(entity, Guid.NewGuid());
            return entity;
        }

        public static List<T> DetachEntities<T>(this ExercisesContext db, List<T> entities) where T : class
        {
            foreach (var entity in entities)
            {
                db.DetachEntity(entity);
            }
            return entities;
        }
        
        public static void AddExerciseToReferencedCollection(this ExercisesContext context, Exercise exercise)
        {
            if (exercise.ExerciseCollectionId == null) return;

            var collection = context
                .ExerciseCollections
                .GetExerciseCollectionById(exercise.ExerciseCollectionId)
                .Result;

            if (collection == null)
            {
                throw new Exception($"The exercise collection with id {exercise.ExerciseCollectionId} doesn't exist.");
            }

            collection.Exercises.Add(exercise);
        }

        /// <summary>
        /// Gives a diagram posted inline with its exercise the same owner as the exercise.
        /// </summary>
        /// <remarks>
        /// Only the top-level entity gets stamped by ResourceBase.OnBeforeCreate, so a
        /// nested Diagram reached SaveChanges owned by Guid.Empty and the insert was
        /// rejected by FK_Diagrams_AspNetUsers_UserId — posting an exercise with its
        /// diagram inline always failed with a 500. An exercise that references an existing
        /// diagram by id has no navigation loaded, so this leaves it alone.
        /// </remarks>
        public static void StampOwnedDiagram(this Exercise exercise, Diagram diagram)
        {
            if (diagram == null || diagram.UserId != Guid.Empty)
            {
                return;
            }

            diagram.UserId = exercise.UserId;
            if (diagram.Created == default)
            {
                diagram.Created = exercise.Created;
            }
        }


        //QUERY EXTENSIONS
        public static IQueryable<TResource> ApplyResourceFilters<TResource, TFilter>(this IQueryable<TResource> query,
            TFilter filter)
            where TResource : IEntity
            where TFilter : FilterBase
        {
            if (filter.Name != null)
            {
                query = query
                    .Where(d => d.Name.StartsWith(filter.Name));
            }

            if (filter.Created != null)
            {
                query = query
                    .Where(d => d.Created.Date == filter.Created);
            }

            return query;
        }

        public static IQueryable<ExerciseCollection> IsNotExam(this IQueryable<ExerciseCollection> query)
        {
            return query.Where(c => !(c is Exam));
        }


        public static async Task<ExerciseCollection> GetExerciseCollectionById(this DbSet<ExerciseCollection> dbSet,
            Guid? id, CancellationToken token = default)
        {
            if (id == null)
            {
                return null;
            }

            return await dbSet
                .AsQueryable()
                .IsNotExam()
                .FirstOrDefaultAsync(c => c.Id == id, cancellationToken: token);
        }

        public static Graph ToGraph(this DiagramDefinition diagramDefinition)
        {
            var graph = new Graph();

            foreach (var node in diagramDefinition.Nodes)
            {
                foreach (var outEdge in node.OutEdges)
                {
                    var connectedTo = diagramDefinition.Nodes
                        .Where(n => n.InEdges.Contains(outEdge))
                        .ToList();

                    foreach (var connectedNode in connectedTo)
                    {
                        graph.AddEdge(node.Number, connectedNode.Number);
                    }
                }
            }

            return graph;
        }

    }
}