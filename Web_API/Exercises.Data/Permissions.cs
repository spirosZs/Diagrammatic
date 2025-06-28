using System;
using System.Collections.Generic;
using System.Linq;
using Exercises.Data.Types;

namespace Exercises.Data
{
    public static class Permissions
    {
        private static Dictionary<Type, Dictionary<string, IEnumerable<EntityOperationType>>>
            PermissionsDictionary = new Dictionary<Type, Dictionary<string, IEnumerable<EntityOperationType>>>()
            {
                {typeof(Exam), ExerciseCollection.Permissions()},
                {typeof(ExerciseCollection), ExerciseCollection.Permissions()},
                {typeof(Submission), Submission.Permissions()},
                {typeof(Exercise), Exercise.Permissions()}
            };

        public static AccessResultType OperationAllowed<TEntity>(EntityOperationType operation,
            IEnumerable<string> roles)
        {
            if (roles.Contains(Constants.ROLE_ADMIN))
            {
                return AccessResultType.Allowed;
            }

            var entityPermissions = PermissionsDictionary[typeof(TEntity)];

            if (entityPermissions == null)
            {
                return AccessResultType.Neutral;
            }

            return entityPermissions.CheckPermissions(operation, roles);
        }

        private static AccessResultType CheckPermissions(
            this Dictionary<string, IEnumerable<EntityOperationType>> permissions, EntityOperationType operation,
            IEnumerable<string> roles
        )
        {
            IEnumerable<EntityOperationType> userPermissions = new List<EntityOperationType>();
            foreach (var role in roles)
            {
                if (permissions.ContainsKey(role))
                {
                    userPermissions = userPermissions.Union(permissions[role]);
                }
            }

            return userPermissions.Contains(operation) ? AccessResultType.Allowed : AccessResultType.Forbidden;
        }
    }
}