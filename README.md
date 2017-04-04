# READ ME

This project implements a simple console program that I use in conjunction with TCC/LE from [JPSoft](http://jpsoft.com).

When invoked inside a git repository it will produce a small string that can be used in either the window title of TCC/LE
or as the console prompt string.

To select whether to get the window title or the console prompt, add the `--prompt` option to the program.

The program will consider these items when producing the strings:

* Whether you're in a repository or not
* Whether you're in a bare repository or not
* Whether your HEAD is tracking a branch or is in detached head state
* Whether your branch has a remote branch
* Whether you're ahead or behind the tracked branch, or both
* Any tags applied to the current head

Example prompt strings:

    master $                               // on branch master and up-to-date
	master +1 $                            // 1 commit ahead
	master -2* $                           // 2 commits behind, * = can fast-forward
	master +1/-2 $                         // 1 commit ahead, 2 behind
	#12345678 $                            // detached head state
	master +1/-2 MERGE CONFLICT $          // you got a conflict during the merge
	master +1/-2 MERGE $                   // you've resolved the conflict

Example window-title strings that would accompany the prompts above:

    master -> origin/master
	master [1 ahead] -> origin/master
	master [2 behind, can fast-forward] -> origin/master
	master [1 ahead, 2 behind] -> origin/master
	#12345678 (detached head)
	master [1 ahead, 2 behind] MERGE CONFLICT -> origin/master
	master [1 ahead, 2 behind] MERGE -> origin/master

The states that can be shown after ahead/behind and before the remote branch can be one or more of the following:

* MERGE (in the middle of a merge)
* REBASE (in the middle of a rebase)
* CONFLICT (got conflicts that needs to be resolved)
* BISECT (in the middle of a bisect)
* CHERRY (in the middle of a cherry-picking sequence)

Here's a screenshot of the program in use, with some extra details in the title:

![Screenshot of TCC/LE with GitPrompt][1]

The configuration that produced this for TCC/LE is as follows:

    SET TITLEPROMPT=`%@TRIM[%@IF[%_ELEVATED == 1,[ADMIN]]%USERDOMAIN%\%USERNAME%       %@cwd[]           %@EXECSTR[%DROPBOX%\Tools\GitPrompt\GitPrompt.exe]`
    PROMPT `%@EXECSTR[%DROPBOX%\Tools\GitPrompt\GitPrompt.exe --prompt]$$s`

  [1]: Assets/ReadmeScreenshot.png?raw=true