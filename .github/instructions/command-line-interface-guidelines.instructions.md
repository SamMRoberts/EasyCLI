# Command Line Interface Guidelines

These are what we consider to be the fundamental principles of good CLI design.Traditionally, UNIX commands were written under the assumption they were going to be used primarily by otherprograms. They had more in common with functions in a programming language than with graphical applications.Today, even though many CLI programs are used primarily (or even exclusively) by humans, a lot of theirinteraction design still carries the baggage of the past. It’s time to shed some of this baggage: if a commandis going to be used primarily by humans, it should be designed for humans first. A core tenet of the original UNIX philosophy is the idea that small, simple programs with clean interfaces canbe combined to build larger systems. Rather than stuff more and more features into those programs, you makeprograms that are modular enough to be recombined as needed.In the old days, pipes and shell scripts played a crucial role in the process of composing programs together.Their role might have diminished with the rise of general-purpose interpreted languages, but they certainlyhaven’t gone away. What’s more, large-scale automation—in the form of CI/CD, orchestration and configurationmanagement—has flourished. Making programs composable is just as important as ever.Fortunately, the long-established conventions of the UNIX environment, designed for this exact purpose, stillhelp us today. Standard in/out/err, signals, exit codes and other mechanisms ensure that different programsclick together nicely. Plain, line-based text is easy to pipe between commands. JSON, a much more recentinvention, affords us more structure when we need it, and lets us more easily integrate command-line toolswith the web.Whatever software you’re building, you can be absolutely certain that people will use it in ways you didn’tanticipate. Your software will become a part in a larger system—your only choice is over whether it will be awell-behaved part.Most importantly, designing for composability does not need to be at odds with designing for humans first.Much of the advice in this document is about how to achieve both.

The terminal’s conventions are hardwired into our fingers. We had to pay an upfront cost by learning aboutcommand line syntax, flags, environment variables and so on, but it pays off in long-term efficiency… as longas programs are consistent.Where possible, a CLI should follow patterns that already exist. That’s what makes CLIs intuitive andguessable; that’s what makes users efficient.That being said, sometimes consistency conflicts with ease of use. For example, many long-established UNIXcommands don’t output much information by default, which can cause confusion or worry for people lessfamiliar with the command line.When following convention would compromise a program’s usability, it might be time to break with it—butsuch a decision should be made with care. The terminal is a world of pure information. You could make an argument that information is the interface—andthat, just like with any interface, there’s often too much or too little of it.A command is saying too little when it hangs for several minutes and the user starts to wonder if it’s broken.A command is saying too much when it dumps pages and pages of debugging output, drowning what’s trulyimportant in an ocean of loose detritus. The end result is the same: a lack of clarity, leaving the userconfused and irritated.It can be very difficult to get this balance right, but it’s absolutely crucial if software is to empower andserve its users. When it comes to making functionality discoverable, GUIs have the upper hand. Everything you can do is laid outin front of you on the screen, so you can find what you need without having to learn anything, and perhaps evendiscover things you didn’t know were possible.It is assumed that command-line interfaces are the opposite of this—that you have to remember how to doeverything. The original Macintosh Human Interface Guidelines, published in 1987, recommend “See-and-point(instead of remember-and-type),” as if you could only choose one or the other.These things needn’t be mutually exclusive. The efficiency of using the command-line comes from rememberingcommands, but there’s no reason the commands can’t help you learn and remember.Discoverable CLIs have comprehensive help texts, provide lots of examples, suggest what command to run next,suggest what to do when there is an error. There are lots of ideas that can be stolen from GUIs to make CLIseasier to learn and use, even for power users.Citation: The Design of Everyday Things (Don Norman), Macintosh Human Interface Guidelines

GUI design, particularly in its early days, made heavy use of metaphor: desktops, files, folders, recycle bins.It made a lot of sense, because computers were still trying to bootstrap themselves into legitimacy. The easeof implementation of metaphors was one of the huge advantages GUIs wielded over CLIs. Ironically, though, theCLI has embodied an accidental metaphor all along: it’s a conversation.Beyond the most utterly simple commands, running a program usually involves more than one invocation.Usually, this is because it’s hard to get it right the first time: the user types a command, gets an error,changes the command, gets a different error, and so on, until it works. This mode of learning through repeatedfailure is like a conversation the user is having with the program.Trial-and-error isn’t the only type of conversational interaction, though. There are others:Running one command to set up a tool and then learning what commands to run to actually start using it.Running several commands to set up an operation, and then a final command to run it (e.g. multiple gitadds, followed by a git commit).Exploring a system—for example, doing a lot of cd and ls to get a sense of a directory structure, or gitlog and git show to explore the history of a file.Doing a dry-run of a complex operation before running it for real.Acknowledging the conversational nature of command-line interaction means you can bring relevant techniques tobear on its design. You can suggest possible corrections when user input is invalid, you can make theintermediate state clear when the user is going through a multi-step process, you can confirm for them thateverything looks good before they do something scary.The user is conversing with your software, whether you intended it or not. At worst, it’s a hostileconversation which makes them feel stupid and resentful. At best, it’s a pleasant exchange that speeds them ontheir way with newfound knowledge and a feeling of achievement.Further reading: The Anti-Mac User Interface (Don Gentner and Jakob Nielsen) Robustness is both an objective and a subjective property. Software should be robust, of course: unexpectedinput should be handled gracefully, operations should be idempotent where possible, and so on. But it shouldalso feel robust.You want your software to feel like it isn’t going to fall apart. You want it to feel immediate andresponsive, as if it were a big mechanical machine, not a flimsy plastic “soft switch.”Subjective robustness requires attention to detail and thinking hard about what can go wrong. It’s lots oflittle things: keeping the user informed about what’s happening, explaining what common errors mean, notprinting scary-looking stack traces.As a general rule, robustness can also come from keeping it simple. Lots of special cases and complex codetend to make a program fragile.

Command-line tools are a programmer’s creative toolkit, so they should be enjoyable to use. This doesn’t meanturning them into a video game, or using lots of emoji (though there’s nothing inherently wrong with emoji ). It means giving the user the feeling that you are on their side, that you want them to succeed, that youhave thought carefully about their problems and how to solve them.There’s no list of actions you can take that will ensure they feel this way, although we hope that followingour advice will take you some of the way there. Delighting the user means exceeding their expectations atevery turn, and that starts with empathy.


## Authors

#


## Foreword

#


## Introduction

#


## Philosophy

#


### Human-first design

#


### Simple parts that work together

#


### Consistency across programs

#


### Saying (just) enough

#


### Ease of discovery

#


### Conversation as the norm

#


### Robustness

#


### Empathy

#Arguments and flags


## Foreword


## Introduction


## Philosophy


### Human-first design


### Simple parts that work together


### Consistency across programs


### Saying (just) enough


### Ease of discovery


### Conversation as the norm


### Robustness


### Empathy


### Chaos

Guidelines


### The Basics


### Help


### Documentation


### Output


### Errors


### Interactivity


### Subcommands


### Robustness


### Future-proofing


### Signals and control characters


### Configuration


### Environment variables


### Naming


### Distribution


### Analytics


## Further reading

The world of the terminal is a mess. Inconsistencies are everywhere, slowing us down and making us second-guess ourselves.Yet it’s undeniable that this chaos has been a source of power. The terminal, like the UNIX-descended computingenvironment in general, places very few constraints on what you can build. In that space, all manner ofinvention has bloomed.It’s ironic that this document implores you to follow existing patterns, right alongside advice thatcontradicts decades of command-line tradition. We’re just as guilty of breaking the rules as anyone.The time might come when you, too, have to break the rules. Do so with intention and clarity of purpose.“Abandon a standard when it is demonstrably harmful to productivity or user satisfaction.” — Jef Raskin, TheHumane Interface This is a collection of specific things you can do to make your command-line program better.The first section contains the essential things you need to follow. Get these wrong, and your program will beeither hard to use or a bad CLI citizen.The rest are nice-to-haves. If you have the time and energy to add these things, your program will be a lotbetter than the average program.The idea is that, if you don’t want to think too hard about the design of your program, you don’t have to: justfollow these rules and your program will probably be good. On the other hand, if you’ve thought about it anddetermined that a rule is wrong for your program, that’s fine. (There’s no central authority that will rejectyour program for not following arbitrary rules.)Also—these rules aren’t written in stone. If you disagree with a general rule for good reason, we hope you’llpropose a change.There are a few basic rules you need to follow. Get these wrong, and your program will be either very hard touse, or flat-out broken.Use a command-line argument parsing library where you can. Either your language’s built-in one, or a goodthird-party one. They will normally handle arguments, flag parsing, help text, and even spelling suggestions ina sensible way.Here are some that we like:Multi-platform: docoptBash: argbashGo: Cobra, cliHaskell: optparse-applicativeJava: picocliJulia: ArgParse.jl, Comonicon.jlKotlin: cliktNode: oclifDeno: parseArgsPerl: Getopt::LongPHP: console, CLImatePython: Argparse, Click, TyperRuby: TTYRust: clapSwift: swift-argument-parserReturn zero exit code on success, non-zero on failure. Exit codes are how scripts determine whether a programsucceeded or failed, so you should report this correctly. Map the non-zero exit codes to the most importantfailure modes.Send output to stdout. The primary output for your command should go to stdout. Anything that is machinereadable should also go to stdout—this is where piping sends things by default.Send messaging to stderr. Log messages, errors, and so on should all be sent to stderr. This means that whencommands are piped together, these messages are displayed to the user and not fed into the next command.


Display extensive help text when asked. Display help when passed -h or --help flags. This also applies tosubcommands which might have their own help text.Display concise help text by default. When myapp or myapp subcommand requires arguments to function, and isrun with no arguments, display concise help text.You can ignore this guideline if your program is interactive by default (e.g. npm init).The concise help text should only include:A description of what your program does.One or two example invocations.Descriptions of flags, unless there are lots of them.An instruction to pass the --help flag for more information.jq does this well. When you type jq, it displays an introductory description and an example, then prompts youto pass jq --help for the full listing of flags:


```text
$ jqjq - commandline JSON processor [version 1.6]Usage: jq [options] <jq filter> [file...] jq [options] --args <jq filter> [strings...] jq [options] --jsonargs <jq filter> [JSON_TEXTS...]jq is a tool for processing JSON inputs, applying the given filter toits JSON text inputs and producing the filter's results as JSON onstandard output.The simplest filter is ., which copies jq's input to its outputunmodified (except for formatting, but note that IEEE754 is usedfor number representation internally, with all that that implies).For more advanced filters see the jq(1) manpage ("man jq")and/or https://stedolan.github.io/jqExample: $ echo '{"foo": 0}' | jq . { "foo": 0 }For a listing of options, use jq --help.
Show full help when -h and --help is passed. All of these should show help:
$ myapp$ myapp --help$ myapp -h
Ignore any other flags and arguments that are passed—you should be able to add -h to the end of anything andit should show help. Don’t overload -h.If your program is git-like, the following should also offer help:
$ myapp help$ myapp help subcommand$ myapp subcommand --help$ myapp subcommand -h
Provide a support path for feedback and issues. A website or GitHub link in the top-level help text is common.In help text, link to the web version of the documentation. If you have a specific page or anchor for asubcommand, link directly to that. This is particularly useful if there is more detailed documentation on theweb, or further reading that might explain the behavior of something.Lead with examples. Users tend to use examples over other forms of documentation, so show them first in thehelp page, particularly the common complex uses. If it helps explain what it’s doing and it isn’t too long,show the actual output too.You can tell a story with a series of examples, building your way toward complex uses.If you’ve got loads of examples, put them somewhere else, in a cheat sheet command or a web page. It’s usefulto have exhaustive, advanced examples, but you don’t want to make your help text really long.For more complex use cases, e.g. when integrating with another tool, it might be appropriate to write afully-fledged tutorial.Display the most common flags and commands at the start of the help text. It’s fine to have lots of flags, butif you’ve got some really common ones, display them first. For example, the Git command displays thecommands for getting started and the most commonly used subcommands first:
$ gitusage: git [--version] [--help] [-C <path>] [-c <name>=<value>] [--exec-path[=<path>]] [--html-path] [--man-path] [--info-path] [-p | --paginate | -P | --no-pager] [--no-replace-objects] [--bare] [--git-dir=<path>] [--work-tree=<path>] [--namespace=<name>] <command> [<args>]These are common Git commands used in various situations:start a working area (see also: git help tutorial) clone Clone a repository into a new directory init Create an empty Git repository or reinitialize an existing onework on the current change (see also: git help everyday) add Add file contents to the index mv Move or rename a file, a directory, or a symlink reset Reset current HEAD to the specified state rm Remove files from the working tree and from the indexexamine the history and state (see also: git help revisions) bisect Use binary search to find the commit that introduced a bug grep Print lines matching a pattern log Show commit logs show Show various types of objects status Show the working tree status…
```

Use formatting in your help text. Bold headings make it much easier to scan. But, try to do it in a terminal-independent way so that your users aren’t staring down a wall of escape characters.


```text
$ heroku apps --helplist your appsUSAGE $ heroku appsOPTIONS -A, --all include apps in all teams -p, --personal list apps in personal account when a default team is set -s, --space=space filter by space -t, --team=team team to use --json output in json formatEXAMPLES $ heroku apps === My Apps example example2 === Collaborated Apps theirapp other@owner.nameCOMMANDS apps:create creates a new app apps:destroy permanently destroy an app apps:errors view app errors apps:favorites list favorited apps apps:info show detailed app information apps:join add yourself to a team app apps:leave remove yourself from a team app apps:lock prevent team members from joining an app apps:open open the app in a web browser apps:rename rename an app apps:stacks show the list of available stacks apps:transfer transfer applications to another user or team apps:unlock unlock an app so any team member can join
Note: When heroku apps --help is piped through a pager, the command emits no escape characters.If the user did something wrong and you can guess what they meant, suggest it. For example, brew update jqtells you that you should run brew upgrade jq.You can ask if they want to run the suggested command, but don’t force it on them. For example:
$ heroku pss › Warning: pss is not a heroku command.Did you mean ps? [y/n]:
```

Rather than suggesting the corrected syntax, you might be tempted to just run it for them, as if they’d typedit right in the first place. Sometimes this is the right thing to do, but not always.Firstly, invalid input doesn’t necessarily imply a simple typo—it can often mean the user has made a logicalmistake, or misused a shell variable. Assuming what they meant can be dangerous, especially if the resultingaction modifies state.Secondly, be aware that if you change what the user typed, they won’t learn the correct syntax. In effect,you’re ruling that the way they typed it is valid and correct, and you’re committing to supporting thatindefinitely. Be intentional in making that decision, and document both syntaxes.Further reading: “Do What I Mean”If your command is expecting to have something piped to it and stdin is an interactive terminal, display helpimmediately and quit. This means it doesn’t just hang, like cat. Alternatively, you could print a log messageto stderr. The purpose of help text is to give a brief, immediate sense of what your tool is, what options are available,and how to perform the most common tasks. Documentation, on the other hand, is where you go into full detail.It’s where people go to understand what your tool is for, what it isn’t for, how it works and how to doeverything they might need to do.Provide web-based documentation. People need to be able to search online for your tool’s documentation, and tolink other people to specific parts. The web is the most inclusive documentation format available.Provide terminal-based documentation. Documentation in the terminal has several nice properties: it’s fast toaccess, it stays in sync with the specific installed version of the tool, and it works without an internetconnection.Consider providing man pages. man pages, Unix’s original system of documentation, are still in use today, andmany users will reflexively check man mycmd as a first step when trying to learn about your tool. To makethem easier to generate, you can use a tool like ronn (which can also generate your web docs).However, not everyone knows about man, and it doesn’t run on all platforms, so you should also make sure yourterminal docs are accessible via your tool itself. For example, git and npm make their man pages accessiblevia the help subcommand, so npm help ls is equivalent to man npm-ls.


```text
NPM-LS(1) NPM-LS(1)NAME npm-ls - List installed packagesSYNOPSIS npm ls [[<@scope>/]<pkg> ...] aliases: list, la, llDESCRIPTION This command will print to stdout all the versions of packages that are installed, as well as their dependencies, in a tree-structure. ...
Human-readable output is paramount. Humans come first, machines second. The most simple and straightforwardheuristic for whether a particular output stream (stdout or stderr) is being read by a human is whether ornot it’s a TTY. Whatever language you’re using, it will have a utility or library for doing this (e.g. Python,Node, Go).Further reading on what a TTY is.Have machine-readable output where it does not impact usability. Streams of text is the universal interface inUNIX. Programs typically output lines of text, and programs typically expect lines of text as input, thereforeyou can compose multiple programs together. This is normally done to make it possible to write scripts, butit can also help the usability for humans using programs. For example, a user should be able to pipe output togrep and it should do what they expect.“Expect the output of every program to become the input to another, as yet unknown, program.” — Doug McIlroyIf human-readable output breaks machine-readable output, use --plain to display output in plain, tabular textformat for integration with tools like grep or awk. In some cases, you might need to output information in adifferent way to make it human-readable.For example, if you are displaying a line-based table, you might choose to split a cell into multiple lines,fitting in more information while keeping it within the width of the screen. This breaks the expected behavior
```


### Chaos

# Guidelines #


### The Basics

#


### Help

#


### Documentation

#


### Output

# fitting in more information while keeping it within the width of the screen. This breaks the expected behaviorof there being one piece of data per line, so you should provide a --plain flag for scripts, which disables allsuch manipulation and outputs one record per line.Display output as formatted JSON if --json is passed. JSON allows for more structure than plain text, so itmakes it much easier to output and handle complex data structures. jq is a common tool for working with JSONon the command-line, and there is now a whole ecosystem of tools that output and manipulate JSON.It is also widely used on the web, so by using JSON as the input and output of programs, you can pipe directlyto and from web services using curl.Display output on success, but keep it brief. Traditionally, when nothing is wrong, UNIX commands display nooutput to the user. This makes sense when they’re being used in scripts, but can make commands appear to behanging or broken when used by humans. For example, cp will not print anything, even if it takes a long time.It’s rare that printing nothing at all is the best default behavior, but it’s usually best to err on the side ofless.For instances where you do want no output (for example, when used in shell scripts), to avoid clumsyredirection of stderr to /dev/null, you can provide a -q option to suppress all non-essential output.If you change state, tell the user. When a command changes the state of a system, it’s especially valuable toexplain what has just happened, so the user can model the state of the system in their head—particularly ifthe result doesn’t directly map to what the user requested.For example, git push tells you exactly what it is doing, and what the new state of the remote branch is:


```text
$ git pushEnumerating objects: 18, done.Counting objects: 100% (18/18), done.Delta compression using up to 8 threadsCompressing objects: 100% (10/10), done.Writing objects: 100% (10/10), 2.09 KiB | 2.09 MiB/s, done.Total 10 (delta 8), reused 0 (delta 0), pack-reused 0remote: Resolving deltas: 100% (8/8), completed with 8 local objects.To github.com:replicate/replicate.git + 6c22c90...a2a5217 bfirsh/fix-delete -> bfirsh/fix-delete
Make it easy to see the current state of the system. If your program does a lot of complex state changes andit is not immediately visible in the filesystem, make sure you make this easy to view.For example, git status tells you as much information as possible about the current state of your Gitrepository, and some hints at how to modify the state:
$ git statusOn branch bfirsh/fix-deleteYour branch is up to date with 'origin/bfirsh/fix-delete'.Changes not staged for commit: (use "git add <file>..." to update what will be committed) (use "git restore <file>..." to discard changes in working directory)modified: cli/pkg/cli/rm.gono changes added to commit (use "git add" and/or "git commit -a")
```

Suggest commands the user should run. When several commands form a workflow, suggesting to the usercommands they can run next helps them learn how to use your program and discover new functionality. Forexample, in the git status output above, it suggests commands you can run to modify the state you areviewing.Actions crossing the boundary of the program’s internal world should usually be explicit. This includes thingslike:Reading or writing files that the user didn’t explicitly pass as arguments (unless those files are storinginternal program state, such as a cache).Talking to a remote server, e.g. to download a file.Increase information density—with ASCII art! For example, ls shows permissions in a scannable way. When youfirst see it, you can ignore most of the information. Then, as you learn how it works, you pick out morepatterns over time. -rw-r--r-- 1 root root 68 Aug 22 23:20 resolv.conflrwxrwxrwx 1 root root 13 Mar 14 20:24 rmt -> /usr/sbin/rmtdrwxr-xr-x 4 root root 4.0K Jul 20 14:51 securitydrwxr-xr-x 2 root root 4.0K Jul 20 14:53 selinux-rw-r----- 1 root shadow 501 Jul 20 14:44 shadow-rw-r--r-- 1 root root 116 Jul 20 14:43 shellsdrwxr-xr-x 2 root root 4.0K Jul 20 14:57 skel-rw-r--r-- 1 root root 0 Jul 20 14:43 subgid-rw-r--r-- 1 root root 0 Jul 20 14:43 subuid Use color with intention. For example, you might want to highlight some text so the user notices it, or usered to indicate an error. Don’t overuse it—if everything is a different color, then the color means nothing andonly makes it harder to read.Disable color if your program is not in a terminal or the user requested it. These things should disablecolors:stdout or stderr is not an interactive terminal (a TTY). It’s best to individually check—if you’re pipingstdout to another program, it’s still useful to get colors on stderr.The NO_COLOR environment variable is set and it is not empty (regardless of its value).The TERM environment variable has the value dumb.The user passes the option --no-color.You may also want to add a MYAPP_NO_COLOR environment variable in case users want to disable colorspecifically for your program.Further reading: no-color.org, 12 Factor CLI AppsIf stdout is not an interactive terminal, don’t display any animations. This will stop progress bars turninginto Christmas trees in CI log output.Use symbols and emoji where it makes things clearer. Pictures can be better than words if you need to makeseveral things distinct, catch the user’s attention, or just add a bit of character. Be careful, though—it canbe easy to overdo it and make your program look cluttered or feel like a toy.For example, yubikey-agent uses emoji to addstructure to the output so it isn’t just a wall of text, and a


to draw your attention to an important piece of information:


```text
$ yubikey-agent -setup
The PIN is up to 8 numbers, letters, or symbols. Not just numbers!
The key will be lost if the PIN and PUK are locked after 3 incorrect tries.Choose a new PIN/PUK: Repeat the PIN/PUK:
Retriculating splines …
```

Done! This YubiKey is secured and ready to go. When the YubiKey blinks, touch it to authorize the login. Here's your new shiny SSH public key:ecdsa-sha2-nistp256 AAAAE2VjZHNhLXNoYTItbmlzdHAyNTYAAAAIbmlzdHAyNTYAAABBBCEJ/UwlHnUFXgENO3ifPZd8zoSKMxESxxot4tMgvfXjmRp5G3BGrAnonncE7Aj11pn3SSYgEcrrn2sMyLGpVS0= Remember: everything breaks, have a backup plan for when this YubiKey does. By default, don’t output information that’s only understandable by the creators of the software. If a piece ofoutput serves only to help you (the developer) understand what your software is doing, it almost certainlyshouldn’t be displayed to normal users by default—only in verbose mode.Invite usability feedback from outsiders and people who are new to your project. They’ll help you seeimportant issues that you are too close to the code to notice.Don’t treat stderr like a log file, at least not by default. Don’t print log level labels (ERR, WARN, etc.) orextraneous contextual information, unless in verbose mode.Use a pager (e.g. less) if you are outputting a lot of text. For example, git diff does this by default. Usinga pager can be error-prone, so be careful with your implementation such that you don’t make the experienceworse for the user. Use a pager only if stdin or stdout is an interactive terminal.A good sensible set of options to use for less is less -FIRX. This does not page if the content fills onescreen, ignores case when you search, enables color and formatting, and leaves the contents on the screen whenless quits.There might be libraries in your language that are more robust than piping to less. For example, pypager inPython.

One of the most common reasons to consult documentation is to fix errors. If you can make errors intodocumentation, then this will save the user loads of time.Catch errors and rewrite them for humans. If you’re expecting an error to happen, catch it and rewrite theerror message to be useful. Think of it like a conversation, where the user has done something wrong and theprogram is guiding them in the right direction. Example: “Can’t write to file.txt. You might need to make itwritable by running ‘chmod +w file.txt’.”Signal-to-noise ratio is crucial. The more irrelevant output you produce, the longer it’s going to take the userto figure out what they did wrong. If your program produces multiple errors of the same type, considergrouping them under a single explanatory header instead of printing many similar-looking lines.Consider where the user will look first. Put the most important information at the end of the output. The eyewill be drawn to red text, so use it intentionally and sparingly.If there is an unexpected or unexplainable error, provide debug and traceback information, and instructions onhow to submit a bug. That said, don’t forget about the signal-to-noise ratio: you don’t want to overwhelm theuser with information they don’t understand. Consider writing the debug log to a file instead of printing it tothe terminal.Make it effortless to submit bug reports. One nice thing you can do is provide a URL and have it pre-populateas much information as possible.Further reading: Google: Writing Helpful Error Messages, Nielsen Norman Group: Error-Message Guidelines A note on terminology:Arguments, or args, are positional parameters to a command. For example, the file paths you provide to cpare args. The order of args is often important: cp foo bar means something different from cp bar foo.Flags are named parameters, denoted with either a hyphen and a single-letter name (-r) or a double hyphenand a multiple-letter name (--recursive). They may or may not also include a user-specified value (--filefoo.txt, or --file=foo.txt). The order of flags, generally speaking, does not affect program semantics.Prefer flags to args. It’s a bit more typing, but it makes it much clearer what is going on. It also makes iteasier to make changes to how you accept input in the future. Sometimes when using args, it’s impossible toadd new input without breaking existing behavior or creating ambiguity.Citation: 12 Factor CLI Apps.Have full-length versions of all flags. For example, have both -h and --help. Having the full version isuseful in scripts where you want to be verbose and descriptive, and you don’t have to look up the meaning offlags everywhere.Citation: GNU Coding Standards.Only use one-letter flags for commonly used flags, particularly at the top-level when using subcommands. Thatway you don’t “pollute” your namespace of short flags, forcing you to use convoluted letters and cases forflags you add in the future.Multiple arguments are fine for simple actions against multiple files. For example, rm file1.txt file2.txtfile3.txt. This also makes it work with globbing: rm *.txt.If you’ve got two or more arguments for different things, you’re probably doing something wrong. The exceptionis a common, primary action, where the brevity is worth memorizing. For example, cp <source> <destination>.Citation: 12 Factor CLI Apps.Use standard names for flags, if there is a standard. If another commonly used command uses a flag name, it’sbest to follow that existing pattern. That way, a user doesn’t have to remember two different options (andwhich command it applies to), and users can even guess an option without having to look at the help text.Here’s a list of commonly used options:-a, --all: All. For example, ps, fetchmail.-d, --debug: Show debugging output.-f, --force: Force. For example, rm -f will force the removal of files, even if it thinks it does nothave permission to do it. This is also useful for commands which are doing something destructive thatusually require user confirmation, but you want to force it to do that destructive action in a script.--json: Display JSON output. See the output section.-h, --help: Help. This should only mean help. See the help section.-n, --dry-run: Dry run. Do not run the command, but describe the changes that would occur if the commandwere run. For example, rsync, git add.--no-input: See the interactivity section.-o, --output: Output file. For example, sort, gcc.-p, --port: Port. For example, psql, ssh.-q, --quiet: Quiet. Display less output. This is particularly useful when displaying output for humansthat you might want to hide when running in a script.-u, --user: User. For example, ps, ssh.--version: Version.-v: This can often mean either verbose or version. You might want to use -d for verbose and this forversion, or for nothing to avoid confusion.Make the default the right thing for most users. Making things configurable is good, but most users are notgoing to find the right flag and remember to use it all the time (or alias it). If it’s not the default, you’remaking the experience worse for most of your users.For example, ls has terse default output to optimize for scripts and other historical reasons, but if it weredesigned today, it would probably default to ls -lhF.Prompt for user input. If a user doesn’t pass an argument or flag, prompt for it. (See also: Interactivity)Never require a prompt. Always provide a way of passing input with flags or arguments. If stdin is not aninteractive terminal, skip prompting and just require those flags/args.Confirm before doing anything dangerous. A common convention is to prompt for the user to type y or yes ifrunning interactively, or requiring them to pass -f or --force otherwise.“Dangerous” is a subjective term, and there are differing levels of danger:Mild: A small, local change such as deleting a file. You might want to prompt for confirmation, you mightnot. For example, if the user is explicitly running a command called something like “delete,” you probablydon’t need to ask.Moderate: A bigger local change like deleting a directory, a remote change like deleting a resource of somekind, or a complex bulk modification that can’t be easily undone. You usually want to prompt forconfirmation here. Consider giving the user a way to “dry run” the operation so they can see what’ll happenbefore they commit to it.Severe: Deleting something complex, like an entire remote application or server. You don’t just want toprompt for confirmation here—you want to make it hard to confirm by accident. Consider asking them totype something non-trivial such as the name of the thing they’re deleting. Let them alternatively pass aflag such as --confirm="name-of-thing", so it’s still scriptable.Consider whether there are non-obvious ways to accidentally destroy things. For example, imagine a situationwhere changing a number in a configuration file from 10 to 1 means that 9 things will be implicitly deleted—this should be considered a severe risk, and should be difficult to do by accident.If input or output is a file, support - to read from stdin or write to stdout. This lets the output ofanother command be the input of your command and vice versa, without using a temporary file. For example,tar can extract files from stdin:


```text
$ curl https://example.com/something.tar.gz | tar xvf -
If a flag can accept an optional value, allow a special word like “none”. For example, ssh -F takes anoptional filename of an alternative ssh_config file, and ssh -F none runs SSH with no config file. Don’t justuse a blank value—this can make it ambiguous whether arguments are flag values or arguments.If possible, make arguments, flags and subcommands order-independent. A lot of CLIs, especially those withsubcommands, have unspoken rules on where you can put various arguments. For example a command might have a--foo flag that only works if you put it before the subcommand:
mycmd --foo=1 subcmdworks$ mycmd subcmd --foo=1unknown flag: --foo
This can be very confusing for the user—especially given that one of the most common things users do whentrying to get a command to work is to hit the up arrow to get the last invocation, stick another option on theend, and run it again. If possible, try to make both forms equivalent, although you might run up against thelimitations of your argument parser.Do not read secrets directly from flags. When a command accepts a secret, e.g. via a --password flag, theflag value will leak the secret into ps output and potentially shell history. And, this sort of flag encouragesthe use of insecure environment variables for secrets. (Environment variables are insecure because they canoften be read by other users, their values end up in debug logs, etc.)Consider accepting sensitive data only via files, e.g. with a --password-file flag, or via stdin. A --password-file flag allows a secret to be passed in discreetly, in a wide variety of contexts.(It’s possible to pass a file’s contents into a flag in Bash by using --password $(< password.txt). Thisapproach has the same security problems mentioned above. It’s best avoided.)
```

Only use prompts or interactive elements if stdin is an interactive terminal (a TTY). This is a prettyreliable way to tell whether you’re piping data into a command or whether it’s being run in a script, inwhich case a prompt won’t work and you should throw an error telling the user what flag to pass.If --no-input is passed, don’t prompt or do anything interactive. This allows users an explicit way to disableall prompts in commands. If the command requires input, fail and tell the user how to pass the information asa flag.If you’re prompting for a password, don’t print it as the user types. This is done by turning off echo in theterminal. Your language should have helpers for this.Let the user escape. Make it clear how to get out. (Don’t do what vim does.) If your program hangs on networkI/O etc, always make Ctrl-C still work. If it’s a wrapper around program execution where Ctrl-C can’t quit(SSH, tmux, telnet, etc), make it clear how to do that. For example, SSH allows escape sequences with the ~escape character. If you’ve got a tool that’s sufficiently complex, you can reduce its complexity by making a set ofsubcommands. If you have several tools that are very closely related, you can make them easier to use anddiscover by combining them into a single command (for example, RCS vs. Git).They’re useful for sharing stuff—global flags, help text, configuration, storage mechanisms.Be consistent across subcommands. Use the same flag names for the same things, have similar outputformatting, etc.Use consistent names for multiple levels of subcommand. If a complex piece of software has lots of objectsand operations that can be performed on those objects, it is a common pattern to use two levels of subcommandfor this, where one is a noun and one is a verb. For example, docker container create. Be consistent with theverbs you use across different types of objects.Either noun verb or verb noun ordering works, but noun verb seems to be more common.Further reading: User experience, CLIs, and breaking the world, by John Starich.Don’t have ambiguous or similarly-named commands. For example, having two subcommands called “update” and“upgrade” is quite confusing. You might want to use different words, or disambiguate with extra words.


### Errors

#


### Arguments and flags

#


### Interactivity

#


### Subcommands

# Validate user input. Everywhere your program accepts data from the user, it will eventually be given bad data.Check early and bail out before anything bad happens, and make the errors understandable.Responsive is more important than fast. Print something to the user in <100ms. If you’re making a networkrequest, print something before you do it so it doesn’t hang and look broken.Show progress if something takes a long time. If your program displays no output for a while, it will lookbroken. A good spinner or progress indicator can make a program appear to be faster than it is.Ubuntu 20.04 has a nice progress bar that sticks to the bottom of the terminal.If the progress bar gets stuck in one place for a long time, the user won’t know if stuff is still happening orif the program’s crashed. It’s good to show estimated time remaining, or even just have an animatedcomponent, to reassure them that you’re still working on it.There are many good libraries for generating progress bars. For example, tqdm for Python, schollz/progressbarfor Go, and node-progress for Node.js.Do stuff in parallel where you can, but be thoughtful about it. It’s already difficult to report progress in theshell; doing it for parallel processes is ten times harder. Make sure it’s robust, and that the output isn’tconfusingly interleaved. If you can use a library, do so—this is code you don’t want to write yourself.Libraries like tqdm for Python and schollz/progressbar for Go support multiple progress bars natively.The upside is that it can be a huge usability gain. For example, docker pull’s multiple progress bars offercrucial insight into what’s going on.


```text
$ docker image pull rubyUsing default tag: latestlatest: Pulling from library/ruby6c33745f49b4: Pull complete ef072fc32a84: Extracting [================================================> ] 7.569MB/7.812MBc0afb8e68e0b: Download complete d599c07d28e6: Download complete f2ecc74db11a: Downloading [=======================> ] 89.11MB/192.3MB3568445c8bf2: Download complete b0efebc74f25: Downloading [===========================================> ] 19.88MB/22.88MB9cb1ba6838a0: Download complete
One thing to be aware of: hiding logs behind progress bars when things go well makes it much easier for theuser to understand what’s going on, but if there is an error, make sure you print out the logs. Otherwise, itwill be very hard to debug.Make things time out. Allow network timeouts to be configured, and have a reasonable default so it doesn’thang forever.Make it recoverable. If the program fails for some transient reason (e.g. the internet connection went down),you should be able to hit <up> and <enter> and it should pick up from where it left off.Make it crash-only. This is the next step up from idempotence. If you can avoid needing to do any cleanup afteroperations, or you can defer that cleanup to the next run, your program can exit immediately on failure orinterruption. This makes it both more robust and more responsive.Citation: Crash-only software: More than meets the eye.People are going to misuse your program. Be prepared for that. They will wrap it in scripts, use it on badinternet connections, run many instances of it at once, and use it in environments you haven’t tested in, withquirks you didn’t anticipate. (Did you know macOS filesystems are case-insensitive but also case-preserving?)
In software of any kind, it’s crucial that interfaces don’t change without a lengthy and well-documenteddeprecation process. Subcommands, arguments, flags, configuration files, environment variables: these are allinterfaces, and you’re committing to keeping them working. (Semantic versioning can only excuse so muchchange; if you’re putting out a major version bump every month, it’s meaningless.)Keep changes additive where you can. Rather than modify the behavior of a flag in a backwards-incompatibleway, maybe you can add a new flag—as long as it doesn’t bloat the interface too much. (See also: Prefer flagsto args.)Warn before you make a non-additive change. Eventually, you’ll find that you can’t avoid breaking an interface.Before you do, forewarn your users in the program itself: when they pass the flag you’re looking to deprecate,tell them it’s going to change soon. Make sure there’s a way they can modify their usage today to make itfuture-proof, and tell them how to do it.If possible, you should detect when they’ve changed their usage and not show the warning any more: now theywon’t notice a thing when you finally roll out the change.Changing output for humans is usually OK. The only way to make an interface easy to use is to iterate on it,and if the output is considered an interface, then you can’t iterate on it. Encourage your users to use --plainor --json in scripts to keep output stable (see Output).Don’t have a catch-all subcommand. If you have a subcommand that’s likely to be the most-used one, you mightbe tempted to let people omit it entirely for brevity’s sake. For example, say you have a run command thatwraps an arbitrary shell command:
$ mycmd run echo "hello world"
You could make it so that if the first argument to mycmd isn’t the name of an existing subcommand, youassume the user means run, so they can just type this:
$ mycmd echo "hello world"
This has a serious drawback, though: now you can never add a subcommand named echo—or anything at all—without risking breaking existing usages. If there’s a script out there that uses mycmd echo, it will dosomething entirely different after that user upgrades to the new version of your tool.Don’t allow arbitrary abbreviations of subcommands. For example, say your command has an installsubcommand. When you added it, you wanted to save users some typing, so you allowed them to type any non-ambiguous prefix, like mycmd ins, or even just mycmd i, and have it be an alias for mycmd install. Now you’restuck: you can’t add any more commands beginning with i, because there are scripts out there that assume imeans install.There’s nothing wrong with aliases—saving on typing is good—but they should be explicit and remain stable.Don’t create a “time bomb.” Imagine it’s 20 years from now. Will your command still run the same as it doestoday, or will it stop working because some external dependency on the internet has changed or is no longermaintained? The server most likely to not exist in 20 years is the one that you are maintaining right now.(But don’t build in a blocking call to Google Analytics either.)
```

If a user hits Ctrl-C (the INT signal), exit as soon as possible. Say something immediately, before you startclean-up. Add a timeout to any clean-up code so it can’t hang forever.If a user hits Ctrl-C during clean-up operations that might take a long time, skip them. Tell the user whatwill happen when they hit Ctrl-C again, in case it is a destructive action.For example, when quitting Docker Compose, you can hit Ctrl-C a second time to force your containers to stopimmediately instead of shutting them down gracefully.


```text
$ docker-compose up…^CGracefully stopping... (press Ctrl+C again to force)
Your program should expect to be started in a situation where clean-up has not been run. (See Crash-onlysoftware: More than meets the eye.)
```

Command-line tools have lots of different types of configuration, and lots of different ways to supply it(flags, environment variables, project-level config files). The best way to supply each piece of configurationdepends on a few factors, chief among them specificity, stability and complexity.Configuration generally falls into a few categories:1. 2. 3.Likely to vary from one invocation of the command to the next.Examples:Setting the level of debugging outputEnabling a safe mode or dry run of a programRecommendation: Use flags. Environment variables may or may not be useful as well.Generally stable from one invocation to the next, but not always. Might vary between projects. Definitelyvaries between different users working on the same project.This type of configuration is often specific to an individual computer.Examples:Providing a non-default path to items needed for a program to startSpecifying how or whether color should appear in outputSpecifying an HTTP proxy server to route all requests throughRecommendation: Use flags and probably environment variables too. Users may want to set the variables intheir shell profile so they apply globally, or in .env for a particular project.If this configuration is sufficiently complex, it may warrant a configuration file of its own, butenvironment variables are usually good enough.Stable within a project, for all users.This is the type of configuration that belongs in version control. Files like Makefile, package.json anddocker-compose.yml are all examples of this.Recommendation: Use a command-specific, version-controlled file.Follow the XDG-spec. In 2010 the X Desktop Group, now freedesktop.org, developed a specification for thelocation of base directories where config files may be located. One goal was to limit the proliferation ofdotfiles in a user’s home directory by supporting a general-purpose ~/.config folder. The XDG Base DirectorySpecification (full spec, summary) is supported by yarn, fish, wireshark, emacs, neovim, tmux, and many otherprojects you know and love.If you automatically modify configuration that is not your program’s, ask the user for consent and tell themexactly what you’re doing. Prefer creating a new config file (e.g. /etc/cron.d/myapp) rather than appending toan existing config file (e.g. /etc/crontab). If you have to append or modify to a system-wide config file, usea dated comment in that file to delineate your additions.Apply configuration parameters in order of precedence. Here is the precedence for config parameters, fromhighest to lowest:FlagsThe running shell’s environment variablesProject-level configuration (e.g. .env)User-level configurationSystem wide configuration

Environment variables are for behavior that varies with the context in which a command is run. The“environment” of an environment variable is the terminal session—the context in which the command is running.So, an env var might change each time a command runs, or between terminal sessions on one machine, orbetween instantiations of one project across several machines.Environment variables may duplicate the functionality of flags or configuration parameters, or they may bedistinct from those things. See Configuration for a breakdown of common types of configuration andrecommendations on when environment variables are most appropriate.For maximum portability, environment variable names must only contain uppercase letters, numbers, andunderscores (and mustn’t start with a number). Which means O_O and OWO are the only emoticons that are alsovalid environment variable names.Aim for single-line environment variable values. While multi-line values are possible, they create usabilityissues with the env command.Avoid commandeering widely used names. Here’s a list of POSIX standard env vars.Check general-purpose environment variables for configuration values when possible:NO_COLOR, to disable color (see Output) or FORCE_COLOR to enable it and ignore the detection logicDEBUG, to enable more verbose outputEDITOR, if you need to prompt the user to edit a file or input more than a single lineHTTP_PROXY, HTTPS_PROXY, ALL_PROXY and NO_PROXY, if you’re going to perform network operations (The HTTPlibrary you’re using might already check for these.)SHELL, if you need to open up an interactive session of the user’s preferred shell (If you need to execute ashell script, use a specific interpreter like /bin/sh)TERM, TERMINFO and TERMCAP, if you’re going to use terminal-specific escape sequencesTMPDIR, if you’re going to create temporary filesHOME, for locating configuration filesPAGER, if you want to automatically page outputLINES and COLUMNS, for output that’s dependent on screen size (e.g. tables)Read environment variables from .env where appropriate. If a command defines environment variables that areunlikely to change as long as the user is working in a particular directory, then it should also read them froma local .env file so users can configure it differently for different projects without having to specify themevery time. Many languages have libraries for reading .env files (Rust, Node, Ruby).Don’t use .env as a substitute for a proper configuration file. .env files have a lot of limitations:A .env file is not commonly stored in source control(Therefore, any configuration stored in it has no history)It has only one data type: stringIt lends itself to being poorly organizedIt makes encoding issues easy to introduceIt often contains sensitive credentials & key material that would be better stored more securelyIf it seems like these limitations will hamper usability or security, then a dedicated config file might bemore appropriate.Do not read secrets from environment variables. While environment variables may be convenient for storingsecrets, they have proven too prone to leakage:Exported environment variables are sent to every process, and from there can easily leak into logs or beexfiltratedShell substitutions like curl -H "Authorization: Bearer $BEARER_TOKEN" will leak into globally-readableprocess state. (cURL offers the -H @filename alternative for reading sensitive headers from a file.)Docker container environment variables can be viewed by anyone with Docker daemon access via dockerinspectEnvironment variables in systemd units are globally readable via systemctl showSecrets should only be accepted via credential files, pipes, AF_UNIX sockets, secret management services, oranother IPC mechanism. “Note the obsessive use of abbreviations and avoidance of capital letters; [Unix] is a system invented by peopleto whom repetitive stress disorder is what black lung is to miners. Long names get worn down to three-letternubbins, like stones smoothed by a river.” — Neal Stephenson, In the Beginning was the Command LineThe name of your program is particularly important on the CLI: your users will be typing it all the time, andit needs to be easy to remember and type.Make it a simple, memorable word. But not too generic, or you’ll step on the toes of other commands andconfuse users. For example, both ImageMagick and Windows used the command convert.Use only lowercase letters, and dashes if you really need to. curl is a good name, DownloadURL is not.Keep it short. Users will be typing it all the time. Don’t make it too short: the very shortest commands arebest reserved for the common utilities used all the time, such as cd, ls, ps.Make it easy to type. If you expect people to type your command name all day, make it easy on their hands.A real-world example: long before Docker Compose was docker compose, it was plum. This turned out to be suchan awkward, one-handed hopscotch that it was immediately renamed to fig, which – as well as being shorter –flows much more easily.Further reading: The Poetics of CLI Command Names

If possible, distribute as a single binary. If your language doesn’t compile to binary executables as standard,see if it has something like PyInstaller. If you really can’t distribute as a single binary, use the platform’snative package installer so you aren’t scattering things on disk that can’t easily be removed. Tread lightly onthe user’s computer.If you’re making a language-specific tool, such as a code linter, then this rule doesn’t apply—it’s safe toassume the user has an interpreter for that language installed on their computer.Make it easy to uninstall. If it needs instructions, put them at the bottom of the install instructions—one ofthe most common times people want to uninstall software is right after installing it. Usage metrics can be helpful to understand how users are using your program, how to make it better, and where


### Robustness

#


### Future-proofing

#


### Signals and control characters

#


### Configuration

#


### Environment variables

#


### Naming

#


### Distribution

#


### Analytics

# Usage metrics can be helpful to understand how users are using your program, how to make it better, and whereto focus effort. But, unlike websites, users of the command-line expect to be in control of their environment,and it is surprising when programs do things in the background without telling them.Do not phone home usage or crash data without consent. Users will find out, and they will be angry. Be veryexplicit about what you collect, why you collect it, how anonymous it is and how you go about anonymizing it,and how long you retain it for.Ideally, ask users whether they want to contribute data (“opt-in”). If you choose to do it by default (“opt-out”), then clearly tell users about it on your website or first run, and make it easy to disable.Examples of projects that collect usage statistics:Angular.js collects detailed analytics using Google Analytics, in the name of feature prioritization. Youhave to explicitly opt in. You can change the tracking ID to point to your own Google Analytics property ifyou want to track Angular usage inside your organization.Homebrew sends metrics to Google Analytics and has a nice FAQ detailing their practices.Next.js collects anonymized usage statistics and is enabled by default.Consider alternatives to collecting analytics.Instrument your web docs. If you want to know how people are using your CLI tool, make a set of docsaround the use cases you’d like to understand best, and see how they perform over time. Look at what peoplesearch for within your docs.Instrument your downloads. This can be a rough metric to understand usage and what operating systems yourusers are running.Talk to your users. Reach out and ask people how they’re using your tool. Encourage feedback and featurerequests in your docs and repos, and try to draw out more context from those who submit feedback.Further reading: Open Source Metrics

The Unix Programming Environment, Brian W. Kernighan and Rob PikePOSIX Utility ConventionsProgram Behavior for All Programs, GNU Coding Standards12 Factor CLI Apps, Jeff DickeyCLI Style Guide, Heroku
