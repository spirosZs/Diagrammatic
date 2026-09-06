using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using Exercises.Data;

namespace Exercises.Common.Diagram
{
    public class Graph
    {
        public int FirstNode => AdjacentList.OrderBy(n => n.Key).FirstOrDefault().Key;

        public int LastNode => AdjacentList.OrderByDescending(n => n.Key).FirstOrDefault().Key;

        public ICollection<int> Vertices => AdjacentList.Keys;


        private List<string> _result;

        public IDictionary<int, List<int>> AdjacentList { get; } = new Dictionary<int, List<int>>();

        public Graph()
        {
        }

        public void AddEdge(int from, int to)
        {
            if (AdjacentList.ContainsKey(from))
            {
                AdjacentList[from].Add(to);
            }
            else
            {
                AdjacentList.Add(from, new List<int> {to});
            }
            AddNode(to);
        }

        public void AddNode(int index)
        {
            if (!AdjacentList.ContainsKey(index))
            {
                AdjacentList.Add(index, new List<int>());
            }
        }

        public List<string> GetAllPaths()
        {
            return GetAllPaths(FirstNode, LastNode);
        }

        public List<string> GetAllPaths(int start, int destination)
        {
            _result = new List<string>();
            var isVisited = new Dictionary<int, bool>();
            var pathList = new List<int>();
            GetAllPaths(start, destination, isVisited, pathList, CopyAdj(AdjacentList));
            return _result;
        }

        private void GetAllPaths(int u, int d, IDictionary<int, bool> isVisited, ICollection<int> localPathList,
            IDictionary<int, List<int>> neighbours)
        {
            isVisited[u] = true;

            var newPathList = localPathList.ToList();
            newPathList.Add(u);

            if (u.Equals(d)) //end
            {
                _result.Add(string.Join(" ", newPathList.Select(x => x)));
                isVisited[u] = false;
                return;
            }
            foreach (var i in neighbours[u].ToList())
            {
                if (isVisited.ContainsKey(i) && isVisited[i]) //if visited reset with new start
                {
                    neighbours[u].Remove(i);
                    GetAllPaths(i, d, new Dictionary<int, bool>(), newPathList, CopyAdj(neighbours));
                }
                else
                {
                    GetAllPaths(i, d, isVisited, newPathList, CopyAdj(neighbours));
                }
            }
            
            isVisited[u] = false;
        }
        
        public IEnumerable<int> ShortestPath(int start, int destination)
        {
            var shortestPath = ShortestPathFunction(start);
            return shortestPath(destination);
        }
        
        public Func<int, IEnumerable<int>> ShortestPathFunction(int start)
        {
            var previous = new Dictionary<int, int>();
            var queue = new Queue<int>();
            queue.Enqueue(start);
            while (queue.Count > 0)
            {
                var vertex = queue.Dequeue();

                // Only edge sources are guaranteed to be keys in the adjacency
                // list; a vertex reached as a neighbour may not be. Guard the
                // lookup instead of indexing blindly so a malformed graph does
                // not throw here.
                if (!AdjacentList.TryGetValue(vertex, out var neighbours))
                    continue;

                foreach (var neighbor in neighbours)
                {
                    if (previous.ContainsKey(neighbor))
                        continue;

                    previous[neighbor] = vertex;
                    queue.Enqueue(neighbor);
                }
            }
            Func<int, IEnumerable<int>> shortestPath = destination =>
            {
                var path = new List<int>();
                var current = destination;
                while (!current.Equals(start))
                {
                    path.Add(current);

                    // destination is not reachable from start (a disconnected
                    // or malformed answer graph): return no path instead of
                    // throwing KeyNotFoundException, so the caller can treat
                    // this vertex as contributing no weight / no match.
                    if (!previous.TryGetValue(current, out var prev))
                        return Enumerable.Empty<int>();

                    current = prev;
                }

                path.Add(start);
                path.Reverse();
                return path;
            };
            return shortestPath;
        }

        /// <summary>
        /// Compares two sets of paths, awarding each correct path the credit the teacher
        /// gave it.
        /// </summary>
        /// <remarks>
        /// The unweighted <see cref="ComparePaths(string[],string[])"/> splits the maximum
        /// score evenly over the correct paths and ignores <paramref name="credits"/>
        /// entirely, so an exercise weighted 50/30/20 scored 67 — not 80 — for the 50 and
        /// the 30. Here each matched path is worth exactly what it was given.
        ///
        /// Falls back to the even split when the credits are missing or do not line up with
        /// the paths one-for-one, since there is then no per-path weight to apply.
        /// </remarks>
        /// <param name="input">The paths the student submitted.</param>
        /// <param name="correct">The correct paths, in the teacher's order.</param>
        /// <param name="credits">The credit for each correct path, aligned by index.</param>
        /// <returns>An evaluation of the input based on the correct paths.</returns>
        public static int ComparePaths(string[] input, string[] correct, double[] credits)
        {
            var correctAnswers = correct?.ToList() ?? new List<string>();

            if (credits == null || credits.Length != correctAnswers.Count)
            {
                return ComparePaths(input ?? new string[0], correct ?? new string[0]);
            }

            var answersGiven = input?.ToList() ?? new List<string>();

            // Each correct path can be claimed once, so repeating one that already scored
            // earns nothing the second time.
            var claimed = new bool[correctAnswers.Count];
            var score = 0d;

            foreach (var answerGiven in answersGiven)
            {
                for (var i = 0; i < correctAnswers.Count; i++)
                {
                    if (claimed[i] || correctAnswers[i] != answerGiven)
                    {
                        continue;
                    }

                    claimed[i] = true;
                    score += credits[i];
                    break;
                }
            }

            // A wrong path costs nothing by itself — it simply earns no credit. Only
            // answering more times than there are paths is penalised, which is what stops
            // a student listing every path they can think of and collecting the lot.
            var surplus = answersGiven.Count - correctAnswers.Count;
            if (surplus > 0)
            {
                score -= surplus * Constants.REDUNDANT_ANSWER_NEGATIVE_IMPACT;
            }

            return score < 0 ? 0 : (int) Math.Round(score, MidpointRounding.AwayFromZero);
        }

        /// <summary>
        /// Compares two sets of paths, splitting the maximum score evenly between them.
        /// </summary>
        /// <param name="input"></param>
        /// <param name="correct"></param>
        /// <returns>An evaluation of the input based on the correct paths.</returns>
        public static int ComparePaths(string[] input, string[] correct)
        {
            var answersGiven = input.ToList();
            var correctAnswersInitial = correct.ToList();
            var correctAnswers = correct.ToList();

            var score = Constants.MAX_SCORE;

            while (answersGiven.Count < correctAnswers.Count)
            {
                answersGiven.Add(null);
            }

            foreach (var answerGiven in answersGiven.ToList())
            {
                if (!correctAnswers.Contains(answerGiven))
                {
                    if (correctAnswersInitial.Count > 0)
                    {
                        score -= (Constants.MAX_SCORE / correctAnswersInitial.Count);
                    }
                }
                else
                {
                    correctAnswers.Remove(answerGiven);
                }
            }

            var difference = Math.Abs(answersGiven.Count - correctAnswersInitial.Count);
            for (var i = 0; i < difference; i++)
            {
                score -= Constants.REDUNDANT_ANSWER_NEGATIVE_IMPACT;
            }

            return score < 0 ? 0 : score;
        }

        /// <summary>
        /// This function is used to compare two graphs.
        /// In order for this to work the first and last node of the two graphs must be the same.
        /// </summary>
        /// <param name="input"></param>
        /// <param name="correct"></param>
        /// <returns>An evaluation of the input based on the correct graph</returns>
        public static int CompareGraphs(Graph input, Graph correct)
        {
            var weights = new Dictionary<int, int>();
            var evaluation = new Dictionary<int, int>();
            var vertices = correct.Vertices.Union(input.Vertices).ToList();
            foreach (var vertex in vertices)
            {
                var pathsFromInputGraph = input.Vertices.Contains(vertex)
                    ? input.GetAllPaths(vertex, input.LastNode).ToArray()
                    : new string[] { };

                var pathsFromCorrectGraph = correct.Vertices.Contains(vertex)
                    ? correct.GetAllPaths(vertex, correct.LastNode).ToArray()
                    : new string[] { };

                var score = ComparePaths(pathsFromInputGraph, pathsFromCorrectGraph);
                evaluation.Add(vertex, score);

                var weight = correct.Vertices.Contains(vertex)
                    ? correct.ShortestPath(vertex, correct.LastNode).Count()
                    : input.ShortestPath(vertex, input.LastNode).Count();
                weights.Add(vertex, weight);
            }
            var sum = vertices.Sum(vertex => evaluation[vertex] * weights[vertex]);
            var weightSum = vertices.Sum(vertex => weights[vertex]);

            // No comparable vertices carry any weight (e.g. an empty submission,
            // or every vertex unreachable to the end node): score it as 0
            // instead of dividing by zero.
            return weightSum == 0 ? 0 : sum / weightSum;
        }
        
        private IDictionary<int, List<int>> CopyAdj(IDictionary<int, List<int>> neighbours)
        {
            return neighbours
                .ToDictionary(neighbour => neighbour.Key, neighbour => neighbour.Value.ToList());
        }
    }
}