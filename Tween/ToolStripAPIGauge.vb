﻿Imports System
Imports System.Drawing
Imports System.Windows.Forms

Public Class ToolStripAPIGauge
    Inherits ToolStripControlHost

    Private originalSize As Size

    Public Sub New()
        MyBase.New(New Control())
        Me.AutoToolTip = True
        AddHandler Me.Control.Paint, AddressOf Draw
        AddHandler Me.Control.TextChanged, AddressOf Control_TextChanged
        AddHandler Me.Control.SizeChanged, AddressOf Control_SizeChanged
    End Sub

    Private _gaugeHeight As Integer
    Public Property GaugeHeight As Integer
        Set(ByVal value As Integer)
            Me._gaugeHeight = value
            Me.Control.Refresh()
        End Set
        Get
            Return _gaugeHeight
        End Get
    End Property

    Private _maxCount As Integer = 350
    Public Property MaxCount As Integer
        Set(ByVal value As Integer)
            Me._maxCount = value
            Me.SetText(Me._remainCount, Me._maxCount)
            Me.Control.Refresh()
        End Set
        Get
            Return Me._maxCount
        End Get
    End Property

    Private _remainCount As Integer
    Public Property RemainCount As Integer
        Set(ByVal value As Integer)
            Me._remainCount = value
            Me.SetText(Me._remainCount, Me._maxCount)
            Me.Control.Refresh()
        End Set
        Get
            Return Me._remainCount
        End Get
    End Property

    Private _resetTime As DateTime
    Public Property ResetTime As DateTime
        Set(ByVal value As DateTime)
            Me._resetTime = value
            If Me._resetTime >= DateTime.Now Then
                Me.ToolTipText = "ResetTime " + Me._resetTime.ToString()
            Else
                Me.ToolTipText = "ResetTime ???"
            End If
            Me.Control.Refresh()
        End Set
        Get
            Return Me._resetTime
        End Get
    End Property

    Private Sub Draw(ByVal sender As Object, ByVal e As PaintEventArgs)
        Dim minute As Double = (Me.ResetTime - DateTime.Now).TotalMinutes
        Dim apiGaugeBounds As New Rectangle(0, _
                                            CType((Me.Control.Height - (Me._gaugeHeight * 2)) / 2, Integer), _
                                            CType(e.ClipRectangle.Width * (Me.RemainCount / Me._maxCount), Integer), _
                                            Me._gaugeHeight)
        Dim timeGaugeBounds As New Rectangle(0, _
                                             apiGaugeBounds.Top + Me._gaugeHeight, _
                                             CType(e.ClipRectangle.Width * (minute / 60), Integer), _
                                             Me._gaugeHeight)

        e.Graphics.FillRectangle(Brushes.LightBlue, apiGaugeBounds)
        e.Graphics.FillRectangle(Brushes.LightPink, timeGaugeBounds)
        e.Graphics.DrawString(Me.Control.Text, Me.Control.Font, SystemBrushes.ControlText, 0, CType(timeGaugeBounds.Top - (Me.Control.Font.Height / 2), Single))
    End Sub

    Private Sub Control_TextChanged(ByVal sender As Object, ByVal e As EventArgs)
        RemoveHandler Me.Control.SizeChanged, AddressOf Me.Control_SizeChanged
        Using g As Graphics = Me.Control.CreateGraphics()
            Me.Control.Size = New Size(CType(Math.Max(g.MeasureString(Me.Control.Text, Me.Control.Font).Width, Me.originalSize.Width), Integer), _
                                       Me.Control.Size.Height)
        End Using
        AddHandler Me.Control.SizeChanged, AddressOf Me.Control_SizeChanged
    End Sub

    Private Sub Control_SizeChanged(ByVal sender As Object, ByVal e As EventArgs)
        Me.originalSize = Me.Control.Size
    End Sub

    Private Sub SetText(ByVal remain As Integer, ByVal max As Integer)
        Dim textFormat As String = "API {0}/{1}"

        If Me._remainCount > -1 AndAlso Me._maxCount > -1 Then
            ' 正常
            Me.Control.Text = String.Format(textFormat, Me._remainCount, Me._maxCount)
        ElseIf Me.RemainCount > -1 Then
            ' uppercount不正
            Me.Control.Text = String.Format(textFormat, Me._remainCount, "???")
        ElseIf Me._maxCount < -1 Then
            ' remaincount不正
            Me.Control.Text = String.Format(textFormat, "???", Me._maxCount)
        Else
            ' 両方とも不正
            Me.Control.Text = String.Format(textFormat, "???", "???")
        End If
    End Sub
End Class