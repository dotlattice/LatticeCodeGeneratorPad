using LatticeCodeGenerator;
using LatticeCodeGenerator.Contract;
using LatticeCodeGenerator.Contract.Exceptions;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Ribbon;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;



namespace LatticeCodeGeneratorPad
{
    public partial class MainWindow : RibbonWindow
    {
        private string lastAssemblyDirectory;
        private readonly List<FileInfo> customAssemblyFiles = new List<FileInfo>();

        public MainWindow()
        {
            this.DataContext = this;
            InitializeComponent();
            LoadSettings();
        }

        #region Events

        private void AboutButtonClicked(object sender, RoutedEventArgs e)
        {
            var aboutDialog = new AboutDialog();
            aboutDialog.Owner = this;
            aboutDialog.ShowDialog();
        }

        private void ManageReferencesButtonClicked(object sender, RoutedEventArgs e)
        {
            var manageReferencesDialog = new ManageReferencesDialog(lastAssemblyDirectory, customAssemblyFiles);
            manageReferencesDialog.Owner = this;
            var showDialogResult = manageReferencesDialog.ShowDialog();
            if (showDialogResult.HasValue && showDialogResult.Value)
            {
                customAssemblyFiles.Clear();
                customAssemblyFiles.AddRange(manageReferencesDialog.AssemblyFiles);
                lastAssemblyDirectory = manageReferencesDialog.LastDirectory;
                SaveSettings();
            }
        }

        private void PlayCommandExecuted(object sender, RoutedEventArgs e)
        {
            SaveSettings();

            OutputEditor.Clear();
            SetStatusMessage("Executing...");

            var codeExecuteAppDomain = AppDomain.CreateDomain("latticeCodeExecution");
            try
            {
                ExecuteCodeInAppDomain(codeExecuteAppDomain);
            }
            catch (Exception ex)
            {
                SetStatusMessage("Failed to generate code.");

                var sb = new StringBuilder();
                sb.AppendLine("Compilation error:");
                sb.AppendLine(ex.Message);
                OutputEditor.Text = sb.ToString();

            }
            finally
            {
                AppDomain.Unload(codeExecuteAppDomain);
            }
        }

        private void ExecuteCodeInAppDomain(AppDomain codeExecuteAppDomain)
        {
            var stopwatch = new System.Diagnostics.Stopwatch();
            stopwatch.Start();

            var codeExecutorObject = codeExecuteAppDomain.CreateInstanceAndUnwrap("LatticeCodeGenerator", "LatticeCodeGenerator.ObjectTreeCSharpCodeGenerator");
            if (codeExecutorObject == null)
            {
                throw new NullReferenceException("codeExecutorObject");
            }

            var codeExecutor = (ICodeGenerator)codeExecutorObject;
            codeExecutor.CustomAssemblyFiles = customAssemblyFiles;

            ICollection<ICodeGenerationResult> codeGeneratorResults = null;
            try
            {
                var code = InputEditor.Text;
                codeGeneratorResults = codeExecutor.Execute(code);
            }
            catch (CompilationException ex)
            {
                SetStatusMessage("Compilation error.");

                var sb = new StringBuilder();

                sb.AppendLine("Compilation error:");
                sb.AppendLine(ex.Message);

                OutputEditor.Text = sb.ToString();
                return;
            }
            catch (Exception ex)
            {
                SetStatusMessage("Execution error.");

                var sb = new StringBuilder();
                sb.AppendLine("Execution error:");
                sb.AppendLine(ex.Message);

                OutputEditor.Text = sb.ToString();
                return;
            }

            stopwatch.Stop();
            SetStatusMessage(string.Format("Execute successful ({0})", TimeSpan.FromTicks(stopwatch.ElapsedTicks)));

            if (codeGeneratorResults != null && codeGeneratorResults.Any())
            {
                var outputStringBuilder = new StringBuilder();
                var codeGenerator = new ObjectTreeCSharpCodeGenerator();

                var firstErrorMessage = codeGeneratorResults.FirstOrDefault(x => x.ErrorMessage != null);
                if (firstErrorMessage != null)
                {
                    SetStatusMessage("Code generation error.");
                    outputStringBuilder.AppendFormat("Failed to generate code: {0}", firstErrorMessage.ErrorMessage).AppendLine();
                }
                else
                {
                    foreach (var codeGeneratorResult in codeGeneratorResults)
                    {
                        var objectCode = codeGeneratorResult.OutputCode;
                        outputStringBuilder.AppendLine(objectCode);
                    }
                    OutputEditor.Text = outputStringBuilder.ToString();
                }
            }
        }

        private void WindowClosing(object sender, CancelEventArgs e)
        {
            SaveSettings();
        }

        #endregion

        #region Settings

        private void LoadSettings()
        {
            var loadedInput = LatticeCodeGeneratorPad.Properties.Settings.Default.Input;
            if (!string.IsNullOrWhiteSpace(loadedInput))
            {
                InputEditor.Text = loadedInput;
            }

            var loadedCustomAssemblyFilePaths = LatticeCodeGeneratorPad.Properties.Settings.Default.CustomAssemblyFiles;
            if (loadedCustomAssemblyFilePaths != null)
            {
                var loadedFileInfos = loadedCustomAssemblyFilePaths.Cast<string>().Select(p => new FileInfo(p)).Where(f => f.Exists);
                customAssemblyFiles.AddRange(loadedFileInfos);
            }

            lastAssemblyDirectory = LatticeCodeGeneratorPad.Properties.Settings.Default.LastAssemblyDirectory;
        }

        private void SaveSettings()
        {
            LatticeCodeGeneratorPad.Properties.Settings.Default.Input = InputEditor.Text;

            var customAssemblyFilePathCollection = new System.Collections.Specialized.StringCollection();
            customAssemblyFilePathCollection.AddRange(customAssemblyFiles.Select(f => f.FullName).ToArray());
            LatticeCodeGeneratorPad.Properties.Settings.Default.CustomAssemblyFiles = customAssemblyFilePathCollection;

            LatticeCodeGeneratorPad.Properties.Settings.Default.Save();
        }

        #endregion


        #region Helpers

        private void SetStatusMessage(string statusMessage)
        {
            ProgressTextBlock.Text = statusMessage;
        }

        #endregion
    }
}
