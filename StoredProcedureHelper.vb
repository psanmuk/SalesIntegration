Imports System.Data.SqlClient

Public Class StoredProcedureHelper

    Private ReadOnly _connectionString As String

    Public Sub New(connectionString As String)
        _connectionString = connectionString
    End Sub

    Public Sub SaveTransSummary(saleDate As String,
                                spid As Integer,
                                stkCod As String,
                                salePrice As Decimal,
                                qtyMember As Integer,
                                qtyOther As Integer,
                                qtyShop As Integer,
                                qtyRetail As Integer,
                                qty711 As Integer,
                                qtyYO As Integer,
                                qtyCJ As Integer,
                                amtSale As Decimal,
                                areaID As Integer,
                                qtyTotal As Integer,
                                Optional transaction As SqlTransaction = Nothing)

        ExecuteNonQuery("usp_SaveTransSummary",
                        New SqlParameter("@SaleDate", saleDate),
                        New SqlParameter("@SPID", spid),
                        New SqlParameter("@STKCOD", stkCod),
                        New SqlParameter("@SalePrice", salePrice),
                        New SqlParameter("@QtyMember", qtyMember),
                        New SqlParameter("@QtyOther", qtyOther),
                        New SqlParameter("@QtyShop", qtyShop),
                        New SqlParameter("@QtyRetail", qtyRetail),
                        New SqlParameter("@Qty711", qty711),
                        New SqlParameter("@QtyYO", qtyYO),
                        New SqlParameter("@QtyCJ", qtyCJ),
                        New SqlParameter("@AmtSale", amtSale),
                        New SqlParameter("@AreaID", areaID),
                        New SqlParameter("@QtyTotal", qtyTotal),
                        transaction)
    End Sub

    Public Sub InsertTbTransMT(docID As Integer,
                               mtCode As String,
                               saleDate As String,
                               spid As Integer,
                               qtySale As Integer,
                               salePrice As Decimal,
                               amtSale As Decimal,
                               Optional transaction As SqlTransaction = Nothing)

        ExecuteNonQuery("usp_InsertTbTransMT",
                        New SqlParameter("@DocID", docID),
                        New SqlParameter("@MTCode", mtCode),
                        New SqlParameter("@SaleDate", saleDate),
                        New SqlParameter("@SPID", spid),
                        New SqlParameter("@QtySale", qtySale),
                        New SqlParameter("@SalePrice", salePrice),
                        New SqlParameter("@AmtSale", amtSale),
                        transaction)
    End Sub

    Public Sub InsertTbTransYO(docNum As String,
                               cusCod As String,
                               shipTo As String,
                               saleDate As String,
                               spid As Integer,
                               qtySale As Integer,
                               salePrice As Decimal,
                               amtSale As Decimal,
                               Optional transaction As SqlTransaction = Nothing)

        ExecuteNonQuery("usp_InsertTbTransYO",
                        New SqlParameter("@DocNum", docNum),
                        New SqlParameter("@CUSCOD", cusCod),
                        New SqlParameter("@SHIPTO", shipTo),
                        New SqlParameter("@SaleDate", saleDate),
                        New SqlParameter("@SPID", spid),
                        New SqlParameter("@QtySale", qtySale),
                        New SqlParameter("@SalePrice", salePrice),
                        New SqlParameter("@AmtSale", amtSale),
                        transaction)
    End Sub

    Public Sub InsertTbTransCJ(docNum As String,
                               cusCod As String,
                               shipTo As String,
                               saleDate As String,
                               spid As Integer,
                               qtySale As Integer,
                               salePrice As Decimal,
                               amtSale As Decimal,
                               Optional transaction As SqlTransaction = Nothing)

        ExecuteNonQuery("usp_InsertTbTransCJ",
                        New SqlParameter("@DocNum", docNum),
                        New SqlParameter("@CUSCOD", cusCod),
                        New SqlParameter("@SHIPTO", shipTo),
                        New SqlParameter("@SaleDate", saleDate),
                        New SqlParameter("@SPID", spid),
                        New SqlParameter("@QtySale", qtySale),
                        New SqlParameter("@SalePrice", salePrice),
                        New SqlParameter("@AmtSale", amtSale),
                        transaction)
    End Sub

    Private Sub ExecuteNonQuery(procName As String, params() As SqlParameter, Optional transaction As SqlTransaction = Nothing)
        Dim needToOpen As Boolean = False
        Dim conn As SqlConnection = If(transaction?.Connection, New SqlConnection(_connectionString))

        Try
            If transaction Is Nothing Then
                conn.Open()
                needToOpen = True
            End If

            Using cmd As New SqlCommand(procName, conn)
                cmd.CommandType = CommandType.StoredProcedure
                If transaction IsNot Nothing Then
                    cmd.Transaction = transaction
                End If
                cmd.Parameters.AddRange(params)
                cmd.ExecuteNonQuery()
            End Using

        Finally
            If needToOpen Then
                conn.Close()
            End If
        End Try
    End Sub

End Class
