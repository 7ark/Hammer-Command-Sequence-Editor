using System.Collections.Generic;
using System.IO;

namespace HammerCmdSeqEditor
{
    public static class CommandSequenceEditor
    {
        /// <summary>
        /// Loads the command sequence data into a file and returns it
        /// </summary>
        /// <param name="path">The path to the command sequence file</param>
        /// <returns>The header file containing the data from ComSeq.wc</returns>
        public static Header LoadCommandSequenceFile(string path)
        {
            Header data = new Header();
            using (FileStream stream = new FileStream(path, FileMode.Open))
            {
                BinaryReader reader = new BinaryReader(stream);

                data.signature = ReadChars(ref reader, 31);
                data.version = reader.ReadSingle();
                data.seq_count = reader.ReadUInt32();
                data.sequences = new List<Sequence>();
                for (int i = 0; i < data.seq_count; i++)
                {
                    Sequence seq = new Sequence();
                    seq.name = ReadChars(ref reader, 128);
                    seq.command_count = reader.ReadUInt32();
                    seq.commands = new List<Command>();
                    for (int j = 0; j < seq.command_count; j++)
                    {
                        Command com = new Command();
                        com.is_enabled = reader.ReadInt32();
                        com.special = reader.ReadInt32();
                        com.executable = ReadChars(ref reader, 260);
                        com.args = ReadChars(ref reader, 260);
                        com.is_long_filename = reader.ReadInt32();
                        com.ensure_check = reader.ReadInt32();
                        com.ensure_file = ReadChars(ref reader, 260);
                        com.use_proc_win = reader.ReadInt32();

                        if(data.version == 0.2f)
                        {
                            com.no_wait = reader.ReadInt32();
                        }

                        seq.commands.Add(com);
                    }
                    data.sequences.Add(seq);
                }

                reader.Close();
            }

            return data;
        }

        /// <summary>
        /// Saves the comseq.wc file in the path specified
        /// </summary>
        /// <param name="path">Path to the location of the comseq.wc file</param>
        /// <param name="data">Data you want to save</param>
        public static void SaveCommandSequenceFile(string path, Header data)
        {
            using (FileStream stream = new FileStream(path, FileMode.Create))
            {
                BinaryWriter writer = new BinaryWriter(stream);

                WriteChars(ref writer, data.signature, 31);
                writer.Write(data.version);
                writer.Write(data.seq_count);
                for (int i = 0; i < data.sequences.Count; i++)
                {
                    WriteChars(ref writer, data.sequences[i].name, 128);
                    writer.Write(data.sequences[i].command_count);
                    for (int j = 0; j < data.sequences[i].commands.Count; j++)
                    {
                        Command com = data.sequences[i].commands[j];
                        writer.Write(com.is_enabled);
                        writer.Write(com.special);
                        WriteChars(ref writer, com.executable, 260);
                        WriteChars(ref writer, com.args, 260);
                        writer.Write(com.is_long_filename);
                        writer.Write(com.ensure_check);
                        WriteChars(ref writer, com.ensure_file, 260);
                        writer.Write(com.use_proc_win);
                        if (data.version == 0.2f)
                        {
                            writer.Write(com.no_wait);
                        }
                    }
                }

                writer.Close();
            }
        }

        private static string ReadChars(ref BinaryReader reader, int length)
        {
            string result = string.Empty;

            //We have to do this wack shit, because sometimes it will attempt to read the char, and it will receive random memory and error out.
            bool go = true;
            for (int j = 0; j < length; j++)
            {
                byte b = reader.ReadByte();
                if (b == 0x00)
                {
                    go = false;
                }

                if (go)
                {
                    result += (char)b;
                }
            }

            return result;
        }

        private static void WriteChars(ref BinaryWriter writer, string text, int length)
        {
            string dataToWrite = text;
            for (int i = 0; i < length; i++)
            {
                if (i < dataToWrite.Length)
                {
                    writer.Write(dataToWrite[i]);
                }
                else
                {
                    writer.Write((byte)0x00);
                }
            }
        }
    }

    //Data stored in the files
    public struct Header
    {
        public string signature; //Length 31
        public float version;
        public uint seq_count;
        public List<Sequence> sequences;
    }

    public struct Sequence
    {
        public string name; //Length 128
        public uint command_count; // Number of commands
        public List<Command> commands;
    }

    public struct Command
    {
        public int is_enabled; // 0/1, If command is enabled. NOTE: CS:GO's branch of hammer uses an integer value here instead of a character.
        public int special;
        public string executable; //Length 260 Name of EXE to run.
        public string args; //Length 260 Arguments for executable.
        public int is_long_filename; // Obsolete, but always set to true. Disables MS-DOS 8-char filenames.
        public int ensure_check; // Ensure file post-exists after compilation
        public string ensure_file; //Length 260 File to check exists.
        public int use_proc_win; // Use Process Window (ignored if exectuable = $game_exe).

        // V 0.2+ only:
        public int no_wait;  // Wait for keypress when done compiling.
    }
}
