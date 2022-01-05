using System;
using System.Collections.Generic;
using System.IO;

namespace DBFConverter
{
    /// <summary>
    /// The Dbf class encapsulated a dBASE table (.dbf) file, allowing
    /// reading from disk, writing to disk, enumerating fields and enumerating records.
    /// </summary>
    public class Dbf
    {
        private DbfHeader header;
        private List<DbfField> fields;
        private List<DbfRecord> records;
        private DbfVersion fileversion;


        public Dbf()
        {
            this.header = DbfHeader.CreateHeader(DbfVersion.FoxBaseDBase3NoMemo);
            this.fields = new List<DbfField>();
            this.records = new List<DbfRecord>();
        }

        public List<DbfField> Fields
        {
            get
            {
                return fields;
            }
        }

        public List<DbfRecord> Records
        {
            get
            {
                return records;
            }
        }

        public DbfRecord CreateRecord()
        {
            DbfRecord record = new DbfRecord(fields);
            this.records.Add(record);
            return record;
        }

        public Tuple<List<DbfRecord>, List<DbfField>, DbfVersion> Read(String path)
        {
            // Open stream for reading.
            FileStream stream = File.Open(path, FileMode.Open, FileAccess.Read);
            BinaryReader reader = new BinaryReader(stream);
            try
            {
                ReadHeader(reader);
                byte[] memoData = ReadMemos(path);
                ReadFields(reader);

                // After reading the fields, we move the read pointer to the beginning
                // of the records, as indicated by the "HeaderLength" value in the header.
                stream.Seek(header.HeaderLength, SeekOrigin.Begin);

                ReadRecords(reader, memoData);

                // Close stream.
                reader.Close();
                stream.Close();
            }
            catch(Exception ex)
            {
                WriteToFile(ex.Message);
                // Close stream.
                reader.Close();
                stream.Close();
            }
            //return records;
            return new Tuple<List<DbfRecord>, List<DbfField>, DbfVersion>(records, fields, fileversion);
        }

        private byte[] ReadMemos(string path)
        {
            String memoPath = Path.ChangeExtension(path, "fpt");
            if (!File.Exists(memoPath))
            {
                memoPath = Path.ChangeExtension(path, "dbt");
                if (!File.Exists(memoPath))
                {
                    return null;
                }
            }

            FileStream str = File.Open(memoPath, FileMode.Open, FileAccess.Read);
            BinaryReader memoReader = new BinaryReader(str);
            byte[] memoData = new byte[str.Length];
            memoData = memoReader.ReadBytes((int)str.Length);
            memoReader.Close();
            str.Close();
            return memoData;
        }

        private void ReadHeader(BinaryReader reader)
        {
            // Peek at version number, then try to read correct version header.
            byte versionByte = reader.ReadByte();
            reader.BaseStream.Seek(0, SeekOrigin.Begin);
            DbfVersion version = (DbfVersion)versionByte;
            fileversion = (DbfVersion)versionByte;
            header = DbfHeader.CreateHeader(version);
            header.Read(reader);
        }

        private void ReadFields(BinaryReader reader)
        {
            fields.Clear();

            // Fields are terminated by 0x0d char.
            while (reader.PeekChar() != 0x0d)
            {
                fields.Add(new DbfField(reader));
            }

            // Read fields terminator.
            reader.ReadByte();
        }

        private void ReadRecords(BinaryReader reader, byte[] memoData)
        {
            records.Clear();

            // Records are terminated by 0x1a char (officially), or EOF (also seen).
            while (reader.PeekChar() != 0x1a && reader.PeekChar() != -1)
            {
                records.Add(new DbfRecord(reader, header, fields, memoData));
            }
        }

        public void Write(String path, DbfVersion version = DbfVersion.Unknown)
        {
            // Use version specified. If unknown specified, use current header version.
            if (version != DbfVersion.Unknown) header.Version = version;
            header = DbfHeader.CreateHeader(header.Version);

            FileStream stream = File.Open(path, FileMode.Create, FileAccess.Write);
            BinaryWriter writer = new BinaryWriter(stream);

            header.Write(writer, fields, records);
            WriteFields(writer);
            WriteRecords(writer);

            writer.Close();
            stream.Close();
        }

        private void WriteFields(BinaryWriter writer)
        {
            foreach (DbfField field in fields)
            {
                field.Write(writer);
            }
            // Write field descriptor array terminator.
            writer.Write((byte)0x0d);
        }

        private void WriteRecords(BinaryWriter writer)
        {
            foreach (DbfRecord record in records)
            {
                record.Write(writer);
            }
            // Write EOF character.
            writer.Write((byte)0x1a);
        }


        // Added On DT: 14/02/2019 To Read Only Columns of The Table
        public List<DbfField> ReadColumns(String path)
        {
            // Open stream for reading.
            FileStream stream = File.Open(path, FileMode.Open, FileAccess.Read);
            BinaryReader reader = new BinaryReader(stream);
            try
            {
                ReadHeader(reader);
                byte[] memoData = ReadMemos(path);
                ReadFields(reader);

                // Close stream.
                reader.Close();
                stream.Close();
            }
            catch (Exception ex)
            {
                WriteToFile(ex.Message);
                // Close stream.
                reader.Close();
                stream.Close();
            }
            //return records;
            return fields;
        }


        // Added On DT: 24/01/2019
        public string Write(String path, List<DbfRecord> newrecords, List<DbfField> newfields, DbfVersion version = DbfVersion.Unknown)
        {
            // Use version specified. If unknown specified, use current header version.
            if (version != DbfVersion.Unknown) header.Version = version;
            header = DbfHeader.CreateHeader(header.Version);

            FileStream stream = File.Open(path, FileMode.Create, FileAccess.Write);
            BinaryWriter writer = new BinaryWriter(stream);
            try
            {
                header.Write(writer, newfields, newrecords);
                WriteFields(writer, newfields);
                WriteRecords(writer, newrecords);

                writer.Close();
                stream.Close();
                return "File Updated Successfully !!!";
            }
            catch (Exception ex)
            {
                writer.Close();
                stream.Close();
                return ex.Message;
            }
        }

        private void WriteFields(BinaryWriter writer, List<DbfField> newfields)
        {
            foreach (DbfField field in newfields)
            {
                field.Write(writer);
            }
            // Write field descriptor array terminator.
            writer.Write((byte)0x0d);
        }

        private void WriteRecords(BinaryWriter writer, List<DbfRecord> newrecords)
        {
            foreach (DbfRecord record in newrecords)
            {
                record.Write(writer);
            }
            // Write EOF character.
            writer.Write((byte)0x1a);
        }

        public string Write(String path, List<DbfRecord> newrecords, DbfField fields, int rowIndex, int columnIndex)
        {

            FileStream stream = File.Open(path, FileMode.Open, FileAccess.Write);
            BinaryWriter writer = new BinaryWriter(stream);
            try
            {
                WriteRecords(writer, newrecords, fields, rowIndex, columnIndex);

                writer.Close();
                stream.Close();
                return "File Updated Successfully !!!";
            }
            catch (Exception ex)
            {
                writer.Close();
                stream.Close();
                return ex.Message;
            }
        }

        private void WriteRecords(BinaryWriter writer, List<DbfRecord> newrecords, DbfField fields, int rowIndex, int columnIndex)
        {
            DbfRecord record = newrecords[rowIndex];
            record.Write(writer, fields, rowIndex, columnIndex);
            writer.Write(" ");
            // Write EOF character.
            writer.Write((byte)0x1a);
        }

        public static void WriteToFile(string text)
        {
            try
            {
                string logFolderPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "DBFReader");
                string logFilePath = Path.Combine(logFolderPath, "ServiceLog-" + DateTime.Now.ToString("yyyy-MM-dd") + ".txt");

                if (!Directory.Exists(logFolderPath))
                    Directory.CreateDirectory(logFolderPath);

                if (!File.Exists(logFilePath))
                    File.Create(logFilePath).Dispose();

                using (StreamWriter writer = new StreamWriter(logFilePath, true))
                {
                    writer.WriteLine(string.Format("{0} : " + text, DateTime.Now.ToString("yyyy-MM-dd hh:mm:ss tt")));
                    writer.Close();
                }

                if (Directory.GetFiles(logFolderPath, "*.txt").Length > 30)
                {
                    string[] files = Directory.GetFiles(logFolderPath, "*.txt");
                    foreach (string file in files)
                    {
                        FileInfo info = new FileInfo(file);
                        info.Refresh();
                        if (info.CreationTime <= DateTime.Now.AddDays(-15))
                        {
                            info.Delete();
                        }
                    }
                }
            }
            catch
            {

            }
        }
    }
}
