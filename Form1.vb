Imports System.Net
Imports System.IO
Imports System.Data.SqlClient
Imports System.Web.Script.Serialization

Public Class Form1
    Private Sub btnFetch_Click(sender As Object, e As EventArgs) Handles btnFetch.Click
        Dim saleDate As String = txtSaleDate.Text.Trim()
        Dim spid As Integer = Integer.Parse(txtSPID.Text.Trim())
        Dim url As String = $"http://localhost:3000/api/tb_trans?saledate={saleDate}&spid={spid}"

        Dim client As New WebClient()
        Dim json As String = client.DownloadString(url)

        Dim serializer As New JavaScriptSerializer()
        Dim data = serializer.Deserialize(Of List(Of Dictionary(Of String, Object)))(json)

        Using conn As New SqlConnection("Server=.;Database=YourDB;Trusted_Connection=True;")
            conn.Open()
            Using cmd As New SqlCommand("EXEC UpsertTrans @jsonData", conn)
                cmd.Parameters.AddWithValue("@jsonData", json)
                cmd.ExecuteNonQuery()
            End Using
        End Using

        MessageBox.Show("Data processed successfully!")
    End Sub
End Class
