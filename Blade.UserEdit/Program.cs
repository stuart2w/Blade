using System;
using System.Collections.Generic;
using System.Windows.Forms;
using IO = System.IO;

namespace Blade.UserEdit
{
	static class Program
	{
		/// <summary>
		/// The main entry point for the application.
		/// </summary>
		[STAThread]
		static void Main()
		{
			Application.EnableVisualStyles();
			Application.SetCompatibleTextRenderingDefault(false);
			// 1st argument is path of base data

			Engine engine = new Engine();
			string file;
			if (Environment.GetCommandLineArgs().Length >= 2)
				file = Environment.GetCommandLineArgs()[1];
			else
			{// see if we can locate file in this folder.  This is what StartEditUserWordListInProcess would do if given a name including %lang%
				// but we'd like to detect whether the file exists here
				string culture = System.Globalization.CultureInfo.CurrentUICulture.Name;
				string folder = IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location) + IO.Path.DirectorySeparatorChar;
				if (IO.File.Exists(folder + "blade-" + culture + ".bin"))
					file = folder + "blade-" + culture + ".bin";
				else if (IO.File.Exists(folder + "blade-" + culture.Substring(0, 2) + ".bin"))
					file = folder + "blade-" + culture.Substring(0, 2) + ".bin";
				else
				{
					MessageBox.Show("No data file specified.  Path of installed system vocab must be specified as command line argument.  This EXE is usually launched from within another application (eg SAW) which provides this.", "No data", MessageBoxButtons.OK);
					return;
				}
			}
			engine.Initialise(file, null);
			Form frm = engine.StartEditUserWordListInProcess();
			Application.Run(frm);
			if (frm.DialogResult == DialogResult.OK)
			{
				Environment.ExitCode = 1;
				engine.SaveEditUserWordListChanges();
			}
			engine.CloseLog();
		}
	}
}