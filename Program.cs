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
		static void Add(Dictionary<string,int> names, string name, int toAdd)
		{
			name = name.Replace("ё", "е").Replace(",", "");
			if (string.IsNullOrWhiteSpace(name)) return;
			if (names.ContainsKey(name))
			{
				names[name] += toAdd;
				return;
			}
			var found = names.Keys.Where(z => z.StartsWith(name) || name.StartsWith(z)).FirstOrDefault();

			if (found!=null)
			{
				names[found] += toAdd;
				return;
			}

			var fio = name.Split(' ');
			if (fio.Length == 2)
			{
				var rename = fio[1] + " " + fio[0];
				if (names.ContainsKey(rename))
				{
					names[rename] += toAdd;
					return;
				}
			}

			Console.WriteLine(name);
		}

		[STAThread]
		static void Main(string[] args)
		{
			var names = File.ReadLines("allnames.txt").OrderBy(z => z).ToDictionary(z => z, z => 0);
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

			foreach (var e in games1)
				registrations[e.Key].Scores[0] = e.Value;

			foreach (var e in games2)
				registrations[e.Key].Scores[1] = e.Value;

			foreach (var e in registrations)
			{
				Add(names,e.Value.First, e.Value.Scores.Sum());
				Add(names, e.Value.Second, e.Value.Scores.Sum());
				
			}

			var result = names.OrderBy(z => z.Key).Select(z => z.Value.ToString()).Aggregate((a, b) => a + "\n" + b);

			System.Windows.Forms.Clipboard.SetText(result);
		}

	}
}

