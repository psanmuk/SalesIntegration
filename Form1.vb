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


Imports System.Net.Http
Imports System.Threading.Tasks
Imports Newtonsoft.Json

Public Class Form1
    Private Async Sub btnLoad_Click(sender As Object, e As EventArgs) Handles btnLoad.Click
        Dim saledate As String = "20250615" ' แก้ตามต้องการ
        Dim spid As Integer = 1

        progressBar1.Style = ProgressBarStyle.Marquee
        btnLoad.Enabled = False

        Try
            Dim apiUrl As String = $"http://localhost:3000/api/trans?saledate={saledate}&spid={spid}"
            Using client As New HttpClient()
                Dim response = Await client.GetStringAsync(apiUrl)
                Dim data = JsonConvert.DeserializeObject(Of List(Of TbTrans))(response)

                For Each item In data
                    CallStoredProc(item)
                Next
                MessageBox.Show("Import success")
            End Using
        Catch ex As Exception
            MessageBox.Show($"Error: {ex.Message}")
        Finally
            progressBar1.Style = ProgressBarStyle.Blocks
            btnLoad.Enabled = True
        End Try
    End Sub

    Private Sub CallStoredProc(item As TbTrans)
        Using conn As New SqlClient.SqlConnection("Server=.;Database=SalesDB;Trusted_Connection=True;")
            Using cmd As New SqlClient.SqlCommand("UpsertTrans", conn)
                cmd.CommandType = CommandType.StoredProcedure
                cmd.Parameters.AddWithValue("@SaleDate", item.SaleDate)
                cmd.Parameters.AddWithValue("@SPID", item.SPID)
                cmd.Parameters.AddWithValue("@STKCOD", item.STKCOD)
                cmd.Parameters.AddWithValue("@SalePrice", item.item_price)
                ' Add other parameters ตาม logic ของคุณ
                conn.Open()
                cmd.ExecuteNonQuery()
            End Using
        End Using
    End Sub

    Public Class TbTrans
        Public Property SaleDate As String
        Public Property SPID As Integer
        Public Property STKCOD As String
        Public Property item_price As Decimal
        ' เพิ่ม properties ที่จำเป็น
    End Class
End Class
