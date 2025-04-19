using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;

public static class VerifyDirectories
{
    static readonly MD5 md5 = MD5.Create();

    public static void Main(string[] args)
    {
        if (args.Length != 2)
        {
            Console.Error.WriteLine("Usage: VerifyDirectories dir1 dir2");
            return;
        }

        Verify(args[0], args[1]);
    }

    public static void Verify(string a, string b)
    {
        var da = new DirectoryInfo(a);
        var db = new DirectoryInfo(b);

        Console.Out.WriteLine(string.Format("Comparing: {0} and {1}", da.FullName, db.FullName));

        int differences = CompareDir(da, db, 0);

        Console.WriteLine("--------------------------------");
        Console.WriteLine(string.Format("Found {0} differences", differences));
    }

    private static int CompareDir(DirectoryInfo da, DirectoryInfo db, int indent)
    {
        int differences = 0;

        PrintDirNames(da.Name, db.Name, "", indent);
        differences += CompareFiles(da, db, indent);

        List<string> subsa = da.GetDirectories().Select(fi => fi.Name).ToList();
        List<string> subsb = db.GetDirectories().Select(fi => fi.Name).ToList();
        List<string> superset = new HashSet<string>(subsa).Union(subsb).OrderBy(f => f).ToList();

        int newindent = indent + 2;

        foreach (var name in superset)
        {
            DirectoryInfo dira = da.GetDirectories().FirstOrDefault(di => di.Name == name);
            DirectoryInfo dirb = db.GetDirectories().FirstOrDefault(di => di.Name == name);
            if (dira != null && dirb != null) {
                differences += CompareDir(dira, dirb, newindent);
            } else {
                string aname = dira != null ? dira.Name : "(absent)";
                string bname = dirb != null ? dirb.Name : "(absent)";
                PrintDirNames(aname, bname, "mismatch, skipping", newindent);
            }
        }

        return differences;
    }

    private static int CompareFiles (DirectoryInfo da, DirectoryInfo db, int indent)
    {
        int differences = 0;

        List<string> filesa = da.GetFiles().Select(fi => fi.Name).ToList();
        List<string> filesb = db.GetFiles().Select(fi => fi.Name).ToList();
        List<string> superset = new HashSet<string>(filesa).Union(filesb).OrderBy(f => f).ToList();

        foreach (var name in superset)
        {
            FileInfo fa = da.GetFiles().FirstOrDefault(fi => fi.Name == name);
            FileInfo fb = db.GetFiles().FirstOrDefault(fi => fi.Name == name);
            string hasha = HashFileAndPrint(name, fa, indent);
            string hashb = HashFileAndPrint(name, fb, 0);
            bool foundone = fa != null && fb != null && hasha != hashb;
            bool missing = (fa == null || fb == null) && hasha != hashb;
            string result = foundone ? "<<< file difference <<<" : missing ? "mismatch, skipping" : "ok";
            Console.WriteLine(result);
            differences += foundone ? 1 : 0;
        }

        return differences;
    }

    private static string HashFileAndPrint (string name, FileInfo fi, int indent)
    {
        string hashstr = "(absent)";
        if (fi != null)
        {
            var bytes = File.ReadAllBytes(fi.FullName);
            var hash = md5.ComputeHash(bytes);
            hashstr = BitConverter.ToString(hash).Replace("-", "");
        }

        PrintFileNAmes(name, indent, hashstr);

        return hashstr;
    }

    private static void PrintDirNames(string aname, string bname, string result, int indent)
    {
        Console.Write("\n".PadRight(indent));
        Console.Write(("DIR " + aname).PadRight(40).Substring(0, 40));
        Console.Write(" | ");
        Console.Write(("DIR " + bname).PadRight(40).Substring(0, 40));
        Console.WriteLine(" | " + result);
    }

    private static void PrintFileNAmes(string name, int indent, string hashstr)
    {
        Console.Write("".PadRight(indent));
        Console.Write(name.PadRight(23).Substring(0, 23));
        Console.Write(" ");
        Console.Write(hashstr.PadRight(16).Substring(0, 16));
        Console.Write(" | ");
    }
}
