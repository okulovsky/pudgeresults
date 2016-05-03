using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleApplication3
{
	class Program
	{
		static void Add(Dictionary<string,int[]> names, string name, int index, int toAdd)
		{
			name = name.Replace("ё", "е").Replace(",", "");
			if (string.IsNullOrWhiteSpace(name)) return;
			if (names.ContainsKey(name))
			{
				names[name][index] += toAdd;
				return;
			}
			var found = names.Keys.Where(z => z.StartsWith(name) || name.StartsWith(z)).FirstOrDefault();

			if (found!=null)
			{
                names[found][index] += toAdd;
				return;
			}

			var fio = name.Split(' ');
			if (fio.Length == 2)
			{
				var rename = fio[1] + " " + fio[0];
				if (names.ContainsKey(rename))
				{
                    names[rename][index] += toAdd;
					return;
				}
			}

			Console.WriteLine(name);
		}

		[STAThread]
		static void Main(string[] args)
		{
			var names = File.ReadLines("allnames.txt").OrderBy(z => z).Select(z=>z.Replace('ё','е')).ToDictionary(z => z, z => new int[3]);
			var registrations = File
				.ReadLines("registrations.tsv")
				.Skip(1)
				.Select(z => z.Split('\t'))
				.OrderBy(z => z[1])
				.ToDictionary(z => z[1].ToLower(), z => new { First = z[2], Second = z[4], Scores = new int[3] });

			var games1 = File.ReadLines("games1.tsv")
				.Skip(1)
				.Select(z => z.Split('\t'))
				.ToDictionary(z => z[0].ToLower(), z => int.Parse(z[3]));
			var games2 = File.ReadLines("games2.tsv")
				.Skip(1)
				.Select(z => z.Split('\t'))
				.ToDictionary(z => z[0].ToLower(), z => int.Parse(z[5]));
            var games3 = File.ReadAllLines("games3.tsv")
                .Skip(1)
                .Select(z => z.Split('\t'))
                .ToDictionary(z => z[0].ToLower(), z => int.Parse(z[3]));

			foreach (var e in games1)
				registrations[e.Key].Scores[0] = e.Value;

			foreach (var e in games2)
				registrations[e.Key].Scores[1] = e.Value;

            foreach (var e in games3)
                registrations[e.Key].Scores[2] = e.Value;

			foreach (var e in registrations)
                for (int i = 0; i < e.Value.Scores.Length; i++)
                {
                    Add(names, e.Value.First, i, e.Value.Scores[i]);
                    Add(names, e.Value.Second,i, e.Value.Scores[i]);
                }
			var result = names.OrderBy(z => z.Key).Select(z => z.Key+"\t"+z.Value[0]+"\t"+z.Value[1]+"\t"+z.Value[2]).Aggregate((a, b) => a + "\n" + b);

			System.Windows.Forms.Clipboard.SetText(result);
            Console.WriteLine(result);
		}

	}
}

