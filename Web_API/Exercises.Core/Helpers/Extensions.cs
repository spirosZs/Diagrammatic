using Exercises.Common.Diagram;
using Exercises.Data;
using System.Text.Json;

namespace Exercises.Core.Helpers
{
    public static class Extensions
    {
        public static Graph ToGraph(this Diagram diagram)
        {
            //TODO
            return new Graph();
        }

        public static T GetDataObject<T>(this Submission submission)
        {
            var data = submission.Data;
            return JsonSerializer
                .Deserialize<T>(data);
        }
    }
}