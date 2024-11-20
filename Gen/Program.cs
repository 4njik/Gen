using System.ComponentModel.DataAnnotations;
using System.Globalization;
using System.Security.Cryptography;

const int length = 100;
const int popSize = 200;
const double pCrossover = 0.9;
const double pMutation = 0.1;
const int maxGeneration = 1000;


List<int> Create_Gen()
{
    Random random = new Random();
    List<int> gen = new List<int>();
    for (int i = 0; i < length; i++)
    {
        gen.Add(random.Next(0,2));
    }

    return gen;
}

int Fitness(List<int> gen)
{
    int fitness = 0;
    for (int i=0; i < length-1; i++)
    {
        if (gen[i] == 1 & gen[i + 1] == 0)
        {
            fitness++;
        }
    }

    return fitness;
}

List<List<int>> TornamentSelection(List<List<int>> pop, List<int> fit)
{
    List<List<int>> next = new List<List<int>>();
    Random random = new Random();
    while (next.Count != popSize)
    {
        int i1 = random.Next(0, popSize);
        int i2 = random.Next(0, popSize);

        if (i1 == i2) { continue; }

        if (fit[i1] > fit[i2]) { next.Add(pop[i1]); }
        else { next.Add(pop[i2]); }
    }

    return next;
}

List<List<int>> ProportionalSelection(List<List<int>> pop, List<int> fit)
{
    List<List<int>> next = new List<List<int>>();
    List<double> chances = new List<double>();
    Random random = new Random();
    double roll;
    int S = fit.Sum();
    chances.Add(fit[0] / S);
    for (int i = 1; i < popSize; i++)
    {
        chances.Add(fit[i] / S + fit[i-1]);
    }

    while (next.Count < pop.Count)
    {
        roll = random.NextDouble()*S;
        for (int j = 0; j < chances.Count; j++)
        {
            if (roll == chances[j])
            {
                next.Add(pop[j]);
                break;
            }
        }
    }
    return next;
}

void Crossover(ref List<int> n1, ref List<int> n2)
{
    Random random = new Random();
    List<int> a1 = new List<int>();
    List<int> a2 = new List<int>();
    int s = random.Next(2, length - 3);
    for (int i = 0;i < length; i++)
    {
        if (i <= s)
        {
            a1.Add(n1[i]);
            a2.Add(n2[i]);
        }
        else
        {
            a1.Add(n2[i]);
            a2.Add(n1[i]);
        }
    }
    n1 = a1;
    n2 = a2;
} 

void Mutation(ref List<int> child, double indpb)
{
    Random rand = new Random();
    for (int i = 0; i < length; i++)
    {
        if (rand.NextDouble() < indpb)
        {
            if (child[i] == 0)
            {
                child[i] = 1;
            }
            else
            {
                child[i] = 0;
            }
        }
    }
}

List<List<int>> population = new List<List<int>>();
List<int> fitnessValues = new List<int>();

List<int> maxFitnessValues = new List<int>();
List<double> meanFitnessValues = new List<double>();

for (int i = 0; i < popSize; i++)
{
    population.Add(Create_Gen());
}

fitnessValues.Clear();
for (int i = 0; i < popSize; i++)
{
    fitnessValues.Add(Fitness(population[i]));
}

void TornamentSelectionGeneration(List<List<int>> pop, List<int> fit)
{
    int count = 0;
    int maxFitness = 0;
    double meanFitness = 0;

    while (maxFitness != length & count < maxGeneration)
    {

        count++;

        Random rand = new Random();


        pop = TornamentSelection(pop, fit);

        for (int i = 0; i < popSize - 1; i = i + 2)
        {
            List<int> child1 = pop[i];
            List<int> child2 = pop[i + 1];
            if (rand.NextDouble() < pCrossover)
            {
                Crossover(ref child1, ref child2);
                pop[i] = child1;
                pop[i + 1] = child2;
            }
        }

        for (int i = 0; i < popSize; i++)
        {
            if (rand.NextDouble() < pMutation)
            {
                List<int> child = pop[i];
                Mutation(ref child, 1.0 / length);
            }
        }

        fit.Clear();
        for (int i = 0; i < popSize; i++)
        {
            fit.Add(Fitness(pop[i]));
        }

        maxFitness = fit.Max();

    }
    Console.WriteLine("Турнирный отбор:");
    Console.WriteLine($"Кол-во поколений: {count}, лучшая полезность: {maxFitness}");
}


void ProportionalSelectionGeneration(List<List<int>> pop, List<int> fit)
{
    int count = 0;
    int maxFitness = 0;
    double meanFitness = 0;

    while (maxFitness != length & count < maxGeneration)
    {

        count++;

        Random rand = new Random();


        pop = ProportionalSelection(pop, fit);

        for (int i = 0; i < popSize - 1; i = i + 2)
        {
            List<int> child1 = pop[i];
            List<int> child2 = pop[i + 1];
            if (rand.NextDouble() < pCrossover)
            {
                Crossover(ref child1, ref child2);
                pop[i] = child1;
                pop[i + 1] = child2;
            }
        }

        for (int i = 0; i < popSize; i++)
        {
            if (rand.NextDouble() < pMutation)
            {
                List<int> child = pop[i];
                Mutation(ref child, 1.0 / length);
            }
        }

        fit.Clear();
        for (int i = 0; i < popSize; i++)
        {
            fit.Add(Fitness(pop[i]));
        }

        maxFitness = fit.Max();

    }
    Console.WriteLine("Пропорциональный отбор:");
    Console.WriteLine($"Кол-во поколений: {count}, лучшая полезность: {maxFitness}");
}

TornamentSelectionGeneration(population, fitnessValues);
ProportionalSelectionGeneration(population, fitnessValues);