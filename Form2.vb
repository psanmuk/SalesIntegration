Imports System.Net.Http
Imports System.Threading.Tasks
Imports Newtonsoft.Json

Public Class Form1

    Private Async Sub btnLoad_Click(sender As Object, e As EventArgs) Handles btnLoad.Click
        ' เริ่มแสดง ProgressBar
        progressBar1.Style = ProgressBarStyle.Marquee
        progressBar1.Visible = True

        Try
            ' เรียก API (เปลี่ยน URL ให้ตรงกับของคุณ)
            Dim saleDate As String = "20250615"
            Dim spid As Integer = 123
            Dim transList = Await LoadDataFromApiAsync(saleDate, spid)

            ' แสดงผลนิดหน่อย
            MessageBox.Show($"โหลดข้อมูลสำเร็จ: {transList.Count} รายการ")

            ' สามารถ loop เพื่อ Insert หรือ Update ข้อมูลลง SQL Server ได้ที่นี่

        Catch ex As Exception
            MessageBox.Show($"เกิดข้อผิดพลาด: {ex.Message}")
        Finally
            ' ปิด ProgressBar
            progressBar1.Visible = False
        End Try
    End Sub

    Private Async Function LoadDataFromApiAsync(saleDate As String, spid As Integer) As Task(Of List(Of TbTransDto))
        Using client As New HttpClient()
            Dim url As String = $"http://localhost:3000/api/trans?saledate={saleDate}&spid={spid}"

            Dim response As HttpResponseMessage = Await client.GetAsync(url)
            response.EnsureSuccessStatusCode()

            Dim json As String = Await response.Content.ReadAsStringAsync()
            Dim dataList As List(Of TbTransDto) = JsonConvert.DeserializeObject(Of List(Of TbTransDto))(json)

            Return dataList
        End Using
    End Function

End Class

Public Class TbTransDto
    Public Property SPID As Integer
    Public Property SaleDate As String
    Public Property STKCOD As String
    Public Property CUSCOD As String
    Public Property SHIPTO As String
    Public Property item_code As String
    Public Property item_qty As Integer
    Public Property item_price As Decimal
    Public Property item_amt As Decimal
    Public Property item_ref As String
End Class
