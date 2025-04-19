# VerifyDirectories

Windows PowerShell + CS script to check file integrity across two directories. 
Useful after copying to / restoring from backup.


## Installation

Simply copy two files to a location of your choice: VerifyDirectory.ps1 and VerifyDirectory.cs - 
and then call the script like so:
```
> powershell .\VerifyDirectories.ps1 dir1 dir2`
```

## Using with Robocopy

This utility was written specifically for use after doing a backup with Robocopy,
which is a great windows backup utility, but does not check file integrity after copying.

But you can use this script right after robocopy, like so:

```
echo this will mirror files from d: to e:
robocopy "D:\files" "E:\files" * /E /MIR

echo now verifying integrity
powershell .\VerifyDirectories.ps1 "D:\files" "E:\files"
```


## Usage example

Call `powershell .\VerifyDirectories.ps1 dir1 dir2`. Here's an example of what kind of output you can expect:

```
PS D:\work\verify-directories> powershell .\VerifyDirectories.ps1 .\test-dir-a\ .\test-dir-b\
Comparing: D:\work\verify-directories\test-dir-a\ and D:\work\verify-directories\test-dir-b\

DIR test-dir-a                           | DIR test-dir-b                           |
bad file.txt            C64267338407814C | bad file.txt            A0B33B6F30EAD869 | <<< file difference <<<
file 1.txt              933222B19FF3E7EA | file 1.txt              933222B19FF3E7EA | ok
file 2.txt              C5053D4DA03789BF | file 2.txt              C5053D4DA03789BF | ok
file 3.txt              FA1F726044EED39D | file 3.txt              FA1F726044EED39D | ok
file only in a.txt      4124BC0A9335C27F | file only in a.txt      (absent)         | mismatch, skipping
file only in b.txt      (absent)         | file only in b.txt      21AD0BD836B90D08 | mismatch, skipping

 DIR subdir in common                     | DIR subdir in common                     |
  subdir file.txt         8277E0910D750195 | subdir file.txt         8277E0910D750195 | ok

   DIR sub subdir                           | DIR sub subdir                           |
    sub sub file.txt        E1671797C52E15F7 | sub sub file.txt        E1671797C52E15F7 | ok

 DIR subdir only in a                     | DIR (absent)                             | mismatch, skipping

 DIR (absent)                             | DIR subdir only in b                     | mismatch, skipping
--------------------------------
Found 1 differences
```

The last line, `Found ... differences` will summarize how many files have mismatched hashes.

### NOTE

If you get the following error: 
```
.\VerifyDirectories.ps1 : File ...\VerifyDirectories.ps1 cannot be loaded because running
scripts is disabled on this system.
```
that's because Windows default settings prohibit running unsigned PowerShell scripts.

You can disable that by going into your Windows settings:
**Settings > For developers > PowerShell > Change execution policy** 
to allow local PowerShell scripts to run without signing.



