Imports System.ComponentModel
Imports System.IO
Imports System.IO.Compression
Public Class Form1
    Private Sub Button1_Click(sender As Object, e As EventArgs) Handles Button1.Click
        If FolderBrowserDialog1.ShowDialog() = DialogResult.OK Then
            TextBox1.Text = FolderBrowserDialog1.SelectedPath
        End If
    End Sub

    Private Sub Button2_Click(sender As Object, e As EventArgs) Handles Button2.Click
        If FolderBrowserDialog1.ShowDialog() = DialogResult.OK Then
            TextBox2.Text = FolderBrowserDialog1.SelectedPath
            update_listview(TextBox2.Text)
        End If
    End Sub

    Private Sub update_listview(sourceDirectory As String)
        If sourceDirectory <> "" Then
            ' Get a list of all the zip files in the source directory
            Dim files As String() = Directory.GetFiles(sourceDirectory)

            ' Clear the listview
            ListView1.Items.Clear()

            ' Loop through each zip file
            For Each file As String In files
                If Path.GetExtension(file) = ".zip" Then
                    ' Add the zip file to the listview
                    Dim item As New ListViewItem(Path.GetFileNameWithoutExtension(file))
                    item.SubItems.Add(file)
                    item.Tag = file
                    ListView1.Items.Add(item)
                Else

                End If
            Next
        Else
        End If
    End Sub

    Private Sub Form1_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        LoadSettings()
    End Sub

    Private Sub Button3_Click(sender As Object, e As EventArgs) Handles Button3.Click
        ' Set the source and destination directories
        Dim sourceDirectory As String = TextBox1.Text
        Dim destinationDirectory As String = TextBox2.Text

        ' Create the destination directory if it does not exist
        If Not Directory.Exists(destinationDirectory) Then
            Directory.CreateDirectory(destinationDirectory)
        End If

        ' Get a list of all the files in the source directory
        Dim files As String() = Directory.GetFiles(sourceDirectory)

        ' Loop through each file
        ToolStripProgressBar1.Value = 0
        ToolStripProgressBar1.Minimum = 0
        ToolStripProgressBar1.Maximum = files.Length
        For Each fileselect As String In files
            savetolog("Processing file " & fileselect)
            ' Get the file name and extension
            Dim fileName As String = Path.GetFileName(fileselect)
            Dim fileExtension As String = Path.GetExtension(fileselect)
            ' Check if the file is a save file (has a .ark extension)
            If fileExtension = ".ark" Then
                ' Create a new zip file with the same name as the save file
                If File.Exists(TextBox2.Text & fileName & ".zip") Then
                    savetolog("Zip file already exists")
                    Dim result As DialogResult = MessageBox.Show("The file already exists. Do you want to replace it?", "File Exists", MessageBoxButtons.YesNo)
                    If result = DialogResult.Yes Then
                        savetolog("Deleting zip file")
                        ' The user wants to replace the file
                        File.Delete(TextBox2.Text & fileName & ".zip")
                    Else
                        ' The user does not want to replace the file
                        savetolog("Skip zip file")
                        Continue For
                    End If
                End If
                savetolog("Creating Zip file")
                Dim zipFilePath As String = Path.Combine(TextBox2.Text, fileName & ".zip")
                If File.Exists(zipFilePath) Then
                    File.Delete(zipFilePath)
                End If

                Using zip As ZipArchive = ZipFile.Open(zipFilePath, ZipArchiveMode.Create)
                    ' Add the save file to the zip file
                    zip.CreateEntryFromFile(fileselect, fileName)
                End Using
                savetolog("--Successful--")
            Else
                savetolog("This file is not ARK SAVE")
            End If
            ToolStripProgressBar1.Value += 1
        Next
        update_listview(TextBox2.Text)
    End Sub

    Private Sub Button5_Click(sender As Object, e As EventArgs) Handles Button5.Click
        ' Loop through each file
        savetolog("Opening OpenFileDialog")
        OpenFileDialog1.FileName = ""
        OpenFileDialog1.Filter = "ARK File (*.ark)|*.ark"
        OpenFileDialog1.InitialDirectory = TextBox1.Text
        OpenFileDialog1.Multiselect = True
        If OpenFileDialog1.ShowDialog() = DialogResult.OK Then
            ToolStripProgressBar1.Value = 0
            ToolStripProgressBar1.Minimum = 0
            ToolStripProgressBar1.Maximum = OpenFileDialog1.FileNames.Length
            For Each fileselect As String In OpenFileDialog1.FileNames
                savetolog("Processing file " & fileselect)
                ' Get the file name and extension
                Dim fileName As String = Path.GetFileName(fileselect)
                Dim fileExtension As String = Path.GetExtension(fileselect)
                ' Check if the file is a save file (has a .ark extension)
                If fileExtension = ".ark" Then
                    ' Create a new zip file with the same name as the save file
                    If File.Exists(TextBox2.Text & fileName & ".zip") Then
                        savetolog("Zip file already exists")
                        Dim result As DialogResult = MessageBox.Show("The file already exists. Do you want to replace it?", "File Exists", MessageBoxButtons.YesNo)
                        If result = DialogResult.Yes Then
                            savetolog("Deleting zip file")
                            ' The user wants to replace the file
                            File.Delete(TextBox2.Text & fileName & ".zip")
                        Else
                            ' The user does not want to replace the file
                            Continue For
                        End If
                    Else
                    End If
                    savetolog("Creating Zip file")
                    Dim zipFilePath As String = Path.Combine(TextBox2.Text, fileName & ".zip")
                    If File.Exists(zipFilePath) Then
                        File.Delete(zipFilePath)
                    End If

                    Using zip As ZipArchive = ZipFile.Open(zipFilePath, ZipArchiveMode.Create)
                        ' Add the save file to the zip file
                        zip.CreateEntryFromFile(fileselect, fileName)
                    End Using
                    savetolog("--Successful--")
                End If
            Next
            update_listview(TextBox2.Text)
        End If
        ToolStripProgressBar1.Value += 1
    End Sub
    Sub savetolog(msg As String)
        TextBox3.AppendText(DateTime.Now.ToString() & ": " & msg & vbCrLf)
    End Sub

    Private Sub Button4_Click(sender As Object, e As EventArgs) Handles Button4.Click
        Dim files As String() = Directory.GetFiles(TextBox1.Text)
        ' Loop through each file
        ToolStripProgressBar1.Value = 0
        ToolStripProgressBar1.Minimum = 0
        ToolStripProgressBar1.Maximum = ListView1.SelectedItems.Count + files.Length
        For Each delfile As String In files
            savetolog("Deleting ARK SAVE " & delfile)
            ' Check if the file has the ".ark" extension
            If Path.GetExtension(delfile) = ".ark" Then
                ' Delete the file
                File.Delete(delfile)
                savetolog("--Successful--")
            End If
            ToolStripProgressBar1.Value += 1
        Next
        For Each item As ListViewItem In ListView1.SelectedItems
            savetolog("Extracting zip file " & item.Tag.ToString)
            ZipFile.ExtractToDirectory(item.Tag.ToString, TextBox1.Text, True)
            savetolog("--Successful--")
            ToolStripProgressBar1.Value += 1
        Next
    End Sub

    Private Sub Form1_Closing(sender As Object, e As CancelEventArgs) Handles Me.Closing
        SaveSettings()
    End Sub
    Private Sub LoadSettings()
        Dim filePath As String = "setting.txt"
        If File.Exists(filePath) Then
            Try
                Using reader As New StreamReader(filePath)
                    Dim fileContent As String = reader.ReadToEnd()
                    Dim lines As String() = fileContent.Split(vbCrLf)
                    For Each line As String In lines
                        ' Split the line into the key and value

                        Dim keyValue As String() = line.Split(">")
                        If keyValue.Length = 2 Then
                            Dim key As String = keyValue(0).Trim()
                            Dim value As String = keyValue(1).Trim()

                            ' Set the appropriate TextBox value based on the key
                            Select Case key
                                Case "SavedArksLocal Directory"
                                    TextBox1.Text = value
                                Case "Zip Directory"
                                    TextBox2.Text = value
                                Case Else
                                    ' Ignore any other keys
                            End Select
                        End If
                    Next
                End Using
                If String.IsNullOrEmpty(TextBox1.Text) OrElse String.IsNullOrEmpty(TextBox2.Text) Then
                    Dim result As DialogResult = MessageBox.Show("Setting error you will reset?", "Error", MessageBoxButtons.YesNo)
                    If result = DialogResult.Yes Then
                        File.Delete("setting.txt")
                        GenerateSetting()
                    Else
                        Me.Close()
                    End If
                Else
                    update_listview(TextBox2.Text)
                End If

            Catch ex As Exception
                ' Handle any exceptions here
            End Try
        Else
            ' File does not exist, create it
            GenerateSetting()
        End If
    End Sub

    Private Sub SaveSettings()
        If String.IsNullOrEmpty(TextBox1.Text) OrElse String.IsNullOrEmpty(TextBox2.Text) Then
            ' Display an error message
            MessageBox.Show("Setting can't save. something wrong", "Error")
            Return
        End If

        Dim filePath As String = "setting.txt"

        ' Use a "using" statement to automatically close the file after it is done being used
        Using writer As StreamWriter = New StreamWriter(filePath)
            writer.WriteLine("# ARK MANAGER SAVE BY OBJN #")
            writer.WriteLine("SavedArksLocal Directory > " + TextBox1.Text)
            writer.WriteLine("Zip Directory > " + TextBox2.Text)
            writer.WriteLine("#")
        End Using
    End Sub
    Private Sub GenerateSetting()
        If FolderBrowserDialog1.ShowDialog() = DialogResult.OK Then
            If Directory.Exists(FolderBrowserDialog1.SelectedPath) Then
                TextBox1.Text = FolderBrowserDialog1.SelectedPath
                TextBox2.Text = FolderBrowserDialog1.SelectedPath.Split("SavedArksLocal\")(0) & "ZippedSaves"
                Directory.CreateDirectory(TextBox2.Text)
                SaveSettings()
                Application.Restart()
            Else
                Me.Close()
            End If
        End If
    End Sub

    Private Sub Button6_Click(sender As Object, e As EventArgs)

    End Sub
End Class
