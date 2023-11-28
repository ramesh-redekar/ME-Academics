namespace NQueenGeneticAlgorithm
{
    public enum GeneticAlgorithmCrossOverWay
    {
        SinglePoint,
        Uniformity,
        OrderBasedUniformity
    }
    public enum GeneticSelectionWay
    {
        Standard,
        TournamentSelection,
        RouletteWheelSelection
    }
    

    public class GeneticAlgorithm
    {
        private static int populationSize = 100;
        private static int n = 8;
        private static int generationLength = 10000;
        private static double mutationRate = 0.3;
        private static int rouletteMembersCount = 10;
        private static int tournamentMembersCount = 10;
        private static GeneticAlgorithmCrossOverWay geneticAlgorithmCrossOverWay = GeneticAlgorithmCrossOverWay.SinglePoint;
        private static GeneticSelectionWay geneticSelectionWay = GeneticSelectionWay.RouletteWheelSelection;

        private static int[] FindBestAnswer(List<int[]> population)
        {
            return population.MinBy(p => GeneticAlgorithm.CalculateFitness(p));
        }

        private static int[] ModifyMember(int[] member)
        {
            bool flag = true;
            Random r = new Random();
            for (int i = 0; i < member.Length; i++)
            {
                if (member.Where(b => b == member[i]).Count() > 1)
                {
                    member[i] = r.Next(member.Length);
                    flag = false;
                }
            }
            if (flag)
            {
                return member;
            }
            return ModifyMember(member);
        }

        private static int[] GenerateMember()
        {
            Random random = new Random();
            var member = new int[n];
            for (int j = 0; j < n; j++)
            {
                member[j] = random.Next(n);
            }
            return member;
        }

        private static List<int[]> GeneratePopulation()
        {
            var population = new List<int[]>();
            for (int i = 0; i < populationSize; i++)
            {
                var member = GenerateMember();
                population.Add(member);
            }
            return population;
        }

        public static int CalculateFitness(int[] member)
        {
            int fitness = 0;
            for (int j = 0; j < n; j++)
            {
                for (int k = j + 1; k < n; k++)
                {
                    int[] c1 = new int[2] { j, member[j] };
                    int[] c2 = new int[2] { k, member[k] };
                    if (IsCollide(c1, c2))
                    {
                        fitness++;
                    }
                }
            }
            return fitness;
        }

        public static bool IsCollide(int[] c1, int[] c2)
        {
            var res = false;
            if (c1[1] == c2[1])
            {
                res = true;
            }
            else if (Math.Abs(c1[0] - c2[0]) == Math.Abs(c1[1] - c2[1]))
            {
                res = true;
            }
            return res;
        }

        public static List<int[]> Selection(List<int[]> population)
        {
            List<int[]> selectedPopulation = new List<int[]>();
            if (geneticSelectionWay == GeneticSelectionWay.Standard)
            {
                List<int[]> sortedPopulation = population.OrderBy(m => CalculateFitness(m)).ToList();
                selectedPopulation = sortedPopulation.Take(new Range(0, populationSize)).ToList();
            }
            if (geneticSelectionWay == GeneticSelectionWay.RouletteWheelSelection)
            {
                Random random = new Random();
                for (int k = 0; k < populationSize; k++)
                {
                    var members = new List<int[]>();
                    var roulette = new List<int[]>();
                    for (int i = 0; i < rouletteMembersCount; i++)
                    {
                        var randomMemberIndex = random.Next(population.Count);
                        int[] randomMember = population[randomMemberIndex];
                        members.Add(randomMember);
                    }
                    int sumFitness = members.Select(m => CalculateFitness(m)).Sum();
                    for (int i = 0; i < rouletteMembersCount; i++)
                    {
                        int memberFitness = CalculateFitness(members[i]);
                        for (int j = 0; j < sumFitness - memberFitness; j++)
                        {
                            roulette.Add(members[i]);
                        }
                    }
                    random = new Random();
                    var selectedMemberIndex = random.Next(roulette.Count);
                    var selectedMember = roulette[selectedMemberIndex];
                    selectedPopulation.Add(selectedMember);
                }
            }
            if (geneticSelectionWay == GeneticSelectionWay.TournamentSelection)
            {
                Random random = new Random();
                for (int k = 0; k < populationSize; k++)
                {
                    var members = new List<int[]>();
                    for (int i = 0; i < tournamentMembersCount; i++)
                    {
                        var randomMemberIndex = random.Next(population.Count);
                        int[] randomMember = population[randomMemberIndex];
                        members.Add(randomMember);
                    }
                    var selectedMember = members.MinBy(m => CalculateFitness(m));
                    selectedPopulation.Add(selectedMember);
                }
            }

            return selectedPopulation;
        }

        public static List<int[]> Mutation(List<int[]> population)
        {
            for (int i = 0; i < populationSize * mutationRate; i++)
            {
                Random random = new Random();
                var randomMember = population[random.Next(populationSize)];
                int newValue = random.Next(n);
                int randomIndex = random.Next(n);
                randomMember[randomIndex] = newValue;
            }
            return population;
        }

        public static List<int[]> Crossover(int[] a, int[] b)
        {
            int half = (int)Math.Floor((decimal)n / 2);
            List<int[]> res = new List<int[]>();
            int[] child1 = new int[n];
            int[] child2 = new int[n];

            if (geneticAlgorithmCrossOverWay == GeneticAlgorithmCrossOverWay.SinglePoint)
            {
                for (int i = 0; i < n; i++)
                {
                    if (i < half)
                    {
                        child1[i] = a[i];
                        child2[i] = b[i];
                    }
                    else
                    {
                        child1[i] = b[i];
                        child2[i] = a[i];
                    }
                }
                
            }
            if (geneticAlgorithmCrossOverWay == GeneticAlgorithmCrossOverWay.Uniformity)
            {
                Random random = new Random();
                for (int i = 0; i < n; i++)
                {
                    child1[i] = a[i];
                    child2[i] = b[i];
                    int sign = random.Next(2);
                    if (sign == 1)
                    {
                        int temp = a[i];
                        child1[i] = b[i];
                        child2[i] = temp;
                    }
                }
               
            }
            if (geneticAlgorithmCrossOverWay == GeneticAlgorithmCrossOverWay.OrderBasedUniformity)
            {
                Random random = new Random();
                List<int> aValues = new List<int>();
                List<int> bValues = new List<int>();
                List<int> indexes = new List<int>();
                for (int i = 0; i < n; i++)
                {
                    int sign = random.Next(2);
                    if (sign == 1)
                    {
                        aValues.Add(a[i]);
                        bValues.Add(b[i]);
                        indexes.Add(i);
                    }
                }
                List<int> newOrderAValues = new List<int>();
                List<int> newOrderBValues = new List<int>();
                for (int j = 0; j < b.Length; j++)
                {
                    for (int i = 0; i < aValues.Count; i++)
                    {
                        if (aValues[i] == b[j])
                        {
                            newOrderAValues.Add(aValues[i]);
                        }
                        if (bValues[i] == a[j])
                        {
                            newOrderBValues.Add(bValues[i]);
                        }
                    }
                }
                a.CopyTo(child1, 0);
                b.CopyTo(child2, 0);
                for (int i = 0; i < indexes.Count; i++)
                {
                    child1[indexes[i]] = newOrderAValues.First();
                    newOrderAValues.RemoveAt(0);
                    child2[indexes[i]] = newOrderBValues.First();
                    newOrderBValues.RemoveAt(0);
                }

            }

            res.Add(child1);
            res.Add(child2);
            return res;
        }
        public static List<int[]> Crossover(List<int[]> population)
        {
            for (int i = 0; i < populationSize; i++)
            {
                Random random = new Random();
                int aIndex = random.Next(populationSize);
                int bIndex = random.Next(populationSize);
                var childs = Crossover(population[aIndex], population[bIndex]);
                population.AddRange(childs);
            }
            return population;
        }

        public static List<int[]> StartProcess()
        {
            List<int[]> population = GeneratePopulation();
            for (int j = 0; j < generationLength; j++)
            {
                if (j != 0)
                {
                    population = Mutation(population);
                }
                population = Crossover(population);
                population = Selection(population);
                var bestAnswer = FindBestAnswer(population);
                if (CalculateFitness(bestAnswer) == 0)
                {
                    return population;
                }
            }
            return population;
        }
    }

    internal class Program
    {
        static void Main(string[] args)
        {

            var populations = GeneticAlgorithm.StartProcess();

            var bestAnswerPopulations = populations.MinBy(x => GeneticAlgorithm.CalculateFitness(x));

            PrintChessboard(bestAnswerPopulations);

            Console.WriteLine("");
        }

        static void PrintChessboard(int[] queenPlacements)
        {
            int n = queenPlacements.Length;

            for (int i = 0; i < n; i++)
            {
                for (int j = 0; j < n; j++)
                {
                    // Check if the current cell is a queen placement
                    char cell = queenPlacements[i] == j ? 'Q' : '.';

                    Console.Write(cell + " ");
                }

                Console.WriteLine();
            }
        }
    }
}
