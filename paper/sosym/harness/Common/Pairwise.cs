namespace Harness.Common;

/// <summary>
/// Deterministic pairwise (strength-2) covering array by the IPOG strategy
/// (Lei et al., 2007): grow the array one parameter at a time, first horizontally by choosing
/// the new parameter's value that covers most uncovered pairs in each existing row, then
/// vertically by adding rows for pairs still uncovered. Don't-care cells are finally set to
/// the parameter's preferred (default) value.
/// </summary>
public static class Pairwise
{
    /// <param name="sizes">Number of values of each parameter.</param>
    /// <param name="preferred">Index of the value to use for a don't-care cell.</param>
    /// <returns>Rows of value indices, one per parameter.</returns>
    public static List<int[]> Generate(IReadOnlyList<int> sizes, IReadOnlyList<int> preferred)
    {
        int n = sizes.Count;
        if (n == 0) return new List<int[]> { Array.Empty<int>() };

        // Order parameters by descending size; IPOG's array is smallest that way.
        var order = Enumerable.Range(0, n).OrderByDescending(i => sizes[i]).ThenBy(i => i).ToArray();
        var rows = new List<int[]>();

        if (n == 1)
        {
            for (int v = 0; v < sizes[order[0]]; v++) rows.Add(new[] { v });
        }
        else
        {
            for (int a = 0; a < sizes[order[0]]; a++)
                for (int b = 0; b < sizes[order[1]]; b++)
                    rows.Add(new[] { a, b });
        }

        for (int k = 2; k < n; k++)
        {
            int sk = sizes[order[k]];
            // uncovered[j][vj, vk]
            var uncovered = new bool[k][,];
            int remaining = 0;
            for (int j = 0; j < k; j++)
            {
                uncovered[j] = new bool[sizes[order[j]], sk];
                for (int a = 0; a < sizes[order[j]]; a++)
                    for (int b = 0; b < sk; b++) { uncovered[j][a, b] = true; remaining++; }
            }

            var used = new int[sk];
            var grown = new List<int[]>(rows.Count);
            foreach (var row in rows)
            {
                int best = 0, bestGain = -1;
                for (int v = 0; v < sk; v++)
                {
                    int gain = 0;
                    for (int j = 0; j < k; j++)
                        if (row[j] >= 0 && uncovered[j][row[j], v]) gain++;
                    if (gain > bestGain || (gain == bestGain && used[v] < used[best])) { best = v; bestGain = gain; }
                }
                var next = new int[k + 1];
                Array.Copy(row, next, k);
                next[k] = best;
                used[best]++;
                for (int j = 0; j < k; j++)
                    if (row[j] >= 0 && uncovered[j][row[j], best]) { uncovered[j][row[j], best] = false; remaining--; }
                grown.Add(next);
            }
            rows = grown;

            // Vertical growth.
            for (int j = 0; j < k && remaining > 0; j++)
            {
                for (int a = 0; a < sizes[order[j]]; a++)
                {
                    for (int b = 0; b < sk; b++)
                    {
                        if (!uncovered[j][a, b]) continue;
                        var host = rows.FirstOrDefault(r => r[k] == b && r[j] < 0);
                        if (host is null)
                        {
                            host = Enumerable.Repeat(-1, k + 1).ToArray();
                            host[k] = b;
                            rows.Add(host);
                        }
                        host[j] = a;
                        // Setting host[j] may also cover pairs (j, a) x (m, host[m]) for m < k,
                        // which are pairs among earlier parameters and already covered; and
                        // (j, a) x (k, b), which is the one targeted here.
                        uncovered[j][a, b] = false;
                        remaining--;
                    }
                }
            }
        }

        // Map back to the caller's parameter order, filling don't-cares.
        var result = new List<int[]>(rows.Count);
        foreach (var row in rows)
        {
            var mapped = new int[n];
            for (int p = 0; p < n; p++)
            {
                int v = row[p];
                mapped[order[p]] = v >= 0 ? v : preferred[order[p]];
            }
            result.Add(mapped);
        }
        return result;
    }

    /// <summary>Check that every pair of values of every two parameters occurs in some row.</summary>
    public static bool Covers(IReadOnlyList<int[]> rows, IReadOnlyList<int> sizes)
    {
        int n = sizes.Count;
        for (int i = 0; i < n; i++)
            for (int j = i + 1; j < n; j++)
            {
                var seen = new bool[sizes[i], sizes[j]];
                foreach (var r in rows) seen[r[i], r[j]] = true;
                for (int a = 0; a < sizes[i]; a++)
                    for (int b = 0; b < sizes[j]; b++)
                        if (!seen[a, b]) return false;
            }
        return true;
    }
}
