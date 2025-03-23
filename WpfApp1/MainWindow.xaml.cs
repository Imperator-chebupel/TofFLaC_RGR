﻿using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows.Media;
using Compiler;
using System.Windows.Threading;
using System.Diagnostics;
using System.ComponentModel;
using System;
using System.Collections.Generic;
using System.Windows.Input;
using Microsoft.Win32;

using Path = System.IO.Path;
using Color = System.Windows.Media.Color;
using Clipboard = System.Windows.Forms.Clipboard;
using TextDataFormat = System.Windows.Forms.TextDataFormat;
using MessageBox = System.Windows.Forms.MessageBox;
using Application = System.Windows.Application;
using OpenFileDialog = Microsoft.Win32.OpenFileDialog;

using System.Windows.Forms;

namespace WpfApp1
{
    public partial class MainWindow : Window
    {
        private string fileName;
        private string filePath;
        private string filesFolderPath;
        private DispatcherTimer timer;
        public MainWindow()
        {
            InitializeComponent();
            string baseDirectory = AppDomain.CurrentDomain.BaseDirectory;
            string projectDirectory = Directory.GetParent(Directory.GetParent(baseDirectory).FullName).FullName;
            filesFolderPath = Path.Combine(Directory.GetParent(projectDirectory).FullName, "Files");
            timer = new DispatcherTimer();
            timer.Interval = TimeSpan.FromMilliseconds(200);
            timer.Tick += Timer_Tick;
        }


        private void Timer_Tick(object sender, EventArgs e)
        {
            //timer.Stop();
            //ColorizeRichTextBox(RCB1);
        }

        private void RichTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
        }

        private void MakeNewFile(object sender, RoutedEventArgs e)
        {
            CreateWindow createFileWindow = new CreateWindow();
            createFileWindow.Closed += CreateFileDialog_Closed;
            createFileWindow.ShowDialog();
        }

        private void CreateFileDialog_Closed(object sender, EventArgs e)
        {
            fileName = ((CreateWindow)sender).FileName;
            filePath = Path.Combine(filesFolderPath, fileName);
            if (!Directory.Exists(filesFolderPath))
            {
                try
                {
                    Directory.CreateDirectory(filesFolderPath);
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Ошибка при создании папки Files: " + ex.Message);
                    return;
                }
            }

            try
            {
                File.WriteAllText(filePath, "Hello");
                textEditor.Clear();
                textEditor.Text += "Hello";

                MessageBox.Show("Файл успешно создан: " + fileName);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ошибка при создании файла: " + ex.Message);
            }
        }

        private void SaveFile_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                File.WriteAllText(filePath, textEditor.Text);  // Используем File.WriteAllText
                MessageBox.Show("Сохранено успешно");
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ошибка сохранения: " + ex.Message);
            }
        }

        private void BackButton_Click(object sender, RoutedEventArgs e)
        {
            textEditor.Undo();

        }

        private void ForwardButton_Click(object sender, RoutedEventArgs e)
        {
            textEditor.Redo();
        }

        private void DeleteAll(object sender, RoutedEventArgs e)
        {
            textEditor.Clear();
        }

        private void RCB1_TextChanged(object sender, TextChangedEventArgs e)
        {
            //Dispatcher.InvokeAsync(HighlightLastWord, DispatcherPriority.ContextIdle);
        }


        private void CopyButton_Click(object sender, RoutedEventArgs e)
        {
            string selectedText = textEditor.SelectedText;
            if (!string.IsNullOrEmpty(selectedText))
            {
                Clipboard.SetText(selectedText);
            }
        }

        private void CutButton_Click(object sender, RoutedEventArgs e)
        {
            if (!string.IsNullOrEmpty(textEditor.SelectedText))
            {
                Clipboard.SetText(textEditor.SelectedText);
                textEditor.SelectedText = "";
            }
        }

        private void PasteButton_Click(object sender, RoutedEventArgs e)
        {
            if (Clipboard.ContainsText(TextDataFormat.Rtf))
            {
                try
                {
                    string rtfText = Clipboard.GetText(TextDataFormat.Rtf);
                    textEditor.SelectedText = rtfText;

                }
                catch (Exception ex)
                {
                    MessageBox.Show("Ошибка при вставке RTF: " + ex.Message);
                }
            }
            else if (Clipboard.ContainsText())
            {
                string clipboardText = Clipboard.GetText();
                textEditor.SelectedText = clipboardText;
            }
        }


        private void ExitApp(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void OpenFile_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Multiselect = false;
            openFileDialog.Filter = "Текстовые файлы (*.txt)|*.txt|Все файлы (*.*)|*.*";
            if (openFileDialog.ShowDialog() == true)
            {
                try
                {
                    string filepath = openFileDialog.FileName;
                    string fileContent = File.ReadAllText(filepath);
                    textEditor.Text = "";
                    textEditor.Text += fileContent;
                    filePath = filepath;
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.ToString());
                }
            }
        }

        private void ManualButton_Click(object sender, RoutedEventArgs e)
        {
            TextWindow textWindow = new TextWindow("Справка");
            textWindow.ShowDialog();
        }

        private void AboutProgrammButton_Click(object sender, RoutedEventArgs e)
        {
            TextWindow textWindow = new TextWindow("О программе");
            textWindow.ShowDialog();
        }

        private void TryToExit(object sender, CancelEventArgs e)
        {
            e.Cancel = true;
            ExitWindow confirmationWindow = new ExitWindow();
            if (confirmationWindow.ShowDialog() == true)
            {
                e.Cancel = false;
                Application.Current.Shutdown();
            }
        }


        private void StartButton_Click(object sender, RoutedEventArgs e)
        {
            RCB2.Document.Blocks.Clear();
            List<string> ToWrite = LeksAnalisation.Analyze(textEditor);
            if (ToWrite != null)
                foreach (var token in ToWrite)
                    RCB2.AppendText(token + "\n");
            else
                RCB2.AppendText("Ошибок нет\n");
        }
    }
}