# Hammer Command Sequence Editor
A small C# library for loading and saving data from the CmdSeq.wc file used in Source's Hammer tool.

Very simple to use, simply drop it in your project, and you'll be able to use CommandSequenceEditor.LoadCommandSequenceFile(path) or CommandSequenceEditor.SaveCommandSequenceFile(path, data).

This will give you an easy way to edit this data if you want to add your own sequences or commands to the Hammer editor. The file structure follows a modified version of the shown structures on this page: https://developer.valvesoftware.com/wiki/Command_Sequences

Short example of how to add a new sequence:

```
Header header = CommandSequenceEditor.LoadCommandSequenceFile(pathToCmdSeq);

AddSMATTSequence(ref header);

CommandSequenceEditor.SaveCommandSequenceFile(pathToCmdSeq, header);
```
```
private static void AddSMATTSequence(ref Header header)
{
    for (int i = 0; i < header.sequences.Count; i++)
    {
        if(header.sequences[i].name.Contains("[SMATT] Generate Timing Map"))
        {
            header.sequences.RemoveAt(i);
            header.seq_count--;
            break;
        }
    }

    header.seq_count++;
    header.sequences.Add(new Sequence()
    {
        name = "[SMATT] Generate Timing Map",
        command_count = 1,
        commands = new List<Command>()
        {
            new Command()
            {
                is_enabled = 1,
                special = 0,
                executable = "$exedir/bin/SMATT/SMATT.exe",
                args = "$gamedir $path/$file",
                is_long_filename = 1,
                ensure_check = 0,
                ensure_file = string.Empty,
                use_proc_win = 0,
                no_wait = 0
            }
        }
    });
}

```
