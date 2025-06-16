Imports System.Data.SqlClient
Imports System.Net.Http
Imports System.Threading.Tasks
Imports Newtonsoft.Json

Public Class TransProcessor
    Private connectionString As String = "Server=your_sql_server;Database=your_db;User Id=your_user;Password=your_pass;"

    Public Async Function ProcessTrans(saleDate As String, spid As Integer, stkcod As String) As Task
        Dim apiUrl As String = $"http://your-api-server:3000/api/tbtrans?saledate={saleDate}&spid={spid}&stkcod={stkcod}"
        Dim httpClient As New HttpClient()
        Dim response = Await httpClient.GetStringAsync(apiUrl)

        Dim transList As List(Of TbTrans) = JsonConvert.DeserializeObject(Of List(Of TbTrans))(response)

        Using conn As New SqlConnection(connectionString)
            conn.Open()

            ' Prepare initial Trans record
            Dim cmdInsert As New SqlCommand("
                MERGE INTO Trans AS target
                USING (SELECT @SaleDate AS SaleDate, @SPID AS SPID, @STKCOD AS STKCOD) AS source
                ON target.SaleDate = source.SaleDate AND target.SPID = source.SPID AND target.STKCOD = source.STKCOD
                WHEN MATCHED THEN
                    UPDATE SET SalePrice = @SalePrice, AmtSale = ISNULL(target.AmtSale, 0) + @AmtSale
                WHEN NOT MATCHED THEN
                    INSERT (SaleDate, SPID, STKCOD, SalePrice, AmtSale)
                    VALUES (@SaleDate, @SPID, @STKCOD, @SalePrice, @AmtSale);", conn)

            ' Initial values
            Dim salePrice As Decimal = If(transList.Count > 0, transList(0).item_price, 0)
            Dim amtSale As Decimal = transList.Sum(Function(x) x.item_amt)

            cmdInsert.Parameters.AddWithValue("@SaleDate", saleDate)
            cmdInsert.Parameters.AddWithValue("@SPID", spid)
            cmdInsert.Parameters.AddWithValue("@STKCOD", stkcod)
            cmdInsert.Parameters.AddWithValue("@SalePrice", salePrice)
            cmdInsert.Parameters.AddWithValue("@AmtSale", amtSale)

            cmdInsert.ExecuteNonQuery()

            ' Process each record
            For Each trans In transList
                Dim itemPrefix As String = If(String.IsNullOrEmpty(trans.item_code), "", trans.item_code.Substring(0, 1))

                Select Case itemPrefix
                    Case "1"
                        UpdateTransQty(conn, saleDate, spid, stkcod, "QtyMember", trans.item_qty, trans.item_amt)
                    Case "2"
                        UpdateTransQty(conn, saleDate, spid, stkcod, "QtyOther", trans.item_qty, trans.item_amt)
                    Case "3"
                        UpdateTransQty(conn, saleDate, spid, stkcod, "QtyShop", trans.item_qty, trans.item_amt)
                    Case "4"
                        UpdateTransQty(conn, saleDate, spid, stkcod, "QtyRetail", trans.item_qty, trans.item_amt)
                    Case "6"
                        InsertMT(conn, trans)
                        UpdateTransQty(conn, saleDate, spid, stkcod, "Qty711", trans.item_qty, trans.item_amt)
                    Case "7"
                        InsertYO(conn, trans)
                        UpdateTransQty(conn, saleDate, spid, stkcod, "QtyYO", trans.item_qty, trans.item_amt)
                    Case "8"
                        InsertCJ(conn, trans)
                        UpdateTransQty(conn, saleDate, spid, stkcod, "QtyCJ", trans.item_qty, trans.item_amt)
                End Select
            Next
        End Using
    End Function

    Private Sub UpdateTransQty(conn As SqlConnection, saleDate As String, spid As Integer, stkcod As String, qtyField As String, qty As Integer, amt As Decimal)
        Dim cmd As New SqlCommand($"
            UPDATE Trans
            SET {qtyField} = ISNULL({qtyField},0) + @Qty,
                AmtSale = ISNULL(AmtSale,0) + @Amt
            WHERE SaleDate = @SaleDate AND SPID = @SPID AND STKCOD = @STKCOD", conn)

        cmd.Parameters.AddWithValue("@Qty", qty)
        cmd.Parameters.AddWithValue("@Amt", amt)
        cmd.Parameters.AddWithValue("@SaleDate", saleDate)
        cmd.Parameters.AddWithValue("@SPID", spid)
        cmd.Parameters.AddWithValue("@STKCOD", stkcod)

        cmd.ExecuteNonQuery()
    End Sub

    Private Sub InsertMT(conn As SqlConnection, trans As TbTrans)
        Dim cmd As New SqlCommand("
            INSERT INTO TbTransMT (DocID, MTCode, SaleDate, SPID, QtySale, SalePrice, AmtSale, DOCNUM)
            VALUES (@DocID, @MTCode, @SaleDate, @SPID, @QtySale, @SalePrice, @AmtSale, '')", conn)

        cmd.Parameters.AddWithValue("@DocID", CInt(trans.item_ref))
        cmd.Parameters.AddWithValue("@MTCode", trans.CUSCOD)
        cmd.Parameters.AddWithValue("@SaleDate", trans.SaleDate)
        cmd.Parameters.AddWithValue("@SPID", trans.SPID)
        cmd.Parameters.AddWithValue("@QtySale", trans.item_qty)
        cmd.Parameters.AddWithValue("@SalePrice", trans.item_price)
        cmd.Parameters.AddWithValue("@AmtSale", trans.item_amt)

        cmd.ExecuteNonQuery()
    End Sub

    Private Sub InsertYO(conn As SqlConnection, trans As TbTrans)
        Dim cmd As New SqlCommand("
            INSERT INTO TbTransYO (DocNum, CUSCOD, SHIPTO, SaleDate, SPID, QtySale, SalePrice, AmtSale, Remark)
            VALUES (@DocNum, @CUSCOD, @SHIPTO, @SaleDate, @SPID, @QtySale, @SalePrice, @AmtSale, '')", conn)

        cmd.Parameters.AddWithValue("@DocNum", trans.item_ref)
        cmd.Parameters.AddWithValue("@CUSCOD", trans.CUSCOD)
        cmd.Parameters.AddWithValue("@SHIPTO", trans.SHIPTO)
        cmd.Parameters.AddWithValue("@SaleDate", trans.SaleDate)
        cmd.Parameters.AddWithValue("@SPID", trans.SPID)
        cmd.Parameters.AddWithValue("@QtySale", trans.item_qty)
        cmd.Parameters.AddWithValue("@SalePrice", trans.item_price)
        cmd.Parameters.AddWithValue("@AmtSale", trans.item_amt)

        cmd.ExecuteNonQuery()
    End Sub

    Private Sub InsertCJ(conn As SqlConnection, trans As TbTrans)
        Dim cmd As New SqlCommand("
            INSERT INTO TbTransCJ (DocNum, CUSCOD, SHIPTO, SaleDate, SPID, QtySale, SalePrice, AmtSale, Remark)
            VALUES (@DocNum, @CUSCOD, @SHIPTO, @SaleDate, @SPID, @QtySale, @SalePrice, @AmtSale, '')", conn)

        cmd.Parameters.AddWithValue("@DocNum", trans.item_ref)
        cmd.Parameters.AddWithValue("@CUSCOD", trans.CUSCOD)
        cmd.Parameters.AddWithValue("@SHIPTO", trans.SHIPTO)
        cmd.Parameters.AddWithValue("@SaleDate", trans.SaleDate)
        cmd.Parameters.AddWithValue("@SPID", trans.SPID)
        cmd.Parameters.AddWithValue("@QtySale", trans.item_qty)
        cmd.Parameters.AddWithValue("@SalePrice", trans.item_price)
        cmd.Parameters.AddWithValue("@AmtSale", trans.item_amt)

        cmd.ExecuteNonQuery()
    End Sub
End Class

' Model class
Public Class TbTrans
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
