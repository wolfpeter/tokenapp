using System.Drawing.Printing;
using System.Printing;
using System.Windows;
using System.Windows.Documents;
using System.Windows.Xps;
using ESCPOS_NET;
using FontFamily = System.Windows.Media.FontFamily;
using FontStyle = System.Drawing.FontStyle;
using Point = System.Drawing.Point;

namespace TokenApp.Services;

public class PrintService
{
    public void PrintDocument(string printerName)
    {
        PrintServer printServer = new PrintServer();
        PrintQueue printQueue = new PrintQueue(printServer, printerName);

        FlowDocument receiptDocument = new FlowDocument
        {
            PageWidth = 288,
            PageHeight = 3000,
            PagePadding = new Thickness(15),
            FontFamily = new FontFamily("Arial")
        };

        receiptDocument.Blocks.Add(new Paragraph
            {
                TextAlignment = TextAlignment.Center, Inlines = {new Bold(new Run("e-matrica ellenőrző\nszelvény\ne-vignette control slip")) { FontSize = 18 }}
            });
        
        receiptDocument.Blocks.Add(new Paragraph
            {
                TextAlignment = TextAlignment.Center, Inlines = { new Bold(new Run("Eladói példány / Seller's copy")){ FontSize = 18 }}
            });
        
        receiptDocument.Blocks.Add(new Paragraph
        {
            TextAlignment = TextAlignment.Center,
            Inlines = { new Run("Nem adóügyi bizonylat!\nNot taxation document!"){ FontSize = 14 }}
        });
        
        receiptDocument.Blocks.Add(new Paragraph
        {
            TextAlignment = TextAlignment.Center,
            Inlines = { new Run("Üzleti partner / Business partner\nAPLUS CONSULTING KFT."){ FontSize = 14 }}
        });
        
        receiptDocument.Blocks.Add(new Paragraph
        {
            TextAlignment = TextAlignment.Center,
            Inlines = { new Run("Viszonteladói iroda / Reseller office\nAPLUS HEGYESHALOM\nHATÁRÁTKELŐ BELÉPŐ OLDAL I. HD1"){ FontSize = 14 }}
        });
        
        receiptDocument.Blocks.Add(new Paragraph
        {
            TextAlignment = TextAlignment.Center,
            Inlines = { new Run("9222 HEGYESHALOM, HATÁRÁTKELŐ BELÉPŐ - OLDAL I. -"){ FontSize = 14 }}
        });
        
        receiptDocument.Blocks.Add(new Paragraph
        {
            TextAlignment = TextAlignment.Center,
            Inlines = { new Run("Viszonteladói pénztár / Reseller cash register/terminal ID\nAPLUSHHHD1 - HEGYESHALOM\nHATÁRÁTKELŐ BELÉPŐ - OLDAL I."){ FontSize = 14 }}
        });
        
        receiptDocument.Blocks.Add(new Paragraph
        {
            TextAlignment = TextAlignment.Center,
            Inlines = { new Run("Felhasználó neve / User name / Cashier\nAPLUSHHHD1"){ FontSize = 14 }}
        });
        
        receiptDocument.Blocks.Add(new Table
        {
            CellSpacing = 0,
            Columns =
            {
                new TableColumn() { Width = new GridLength(1, GridUnitType.Auto) },
                new TableColumn() { Width = new GridLength(1, GridUnitType.Auto) }
            }, RowGroups =
            {
                new TableRowGroup{Rows =
                {
                    new TableRow
                    {
                        Cells =
                        {
                            new TableCell(new Paragraph(new Run("NÚSZ Call Center:"))){TextAlignment = TextAlignment.Left, FontSize = 14},
                            new TableCell(new Paragraph(new Run("+36 (36) 587-500"))){TextAlignment = TextAlignment.Right, FontSize = 14}
                        }
                    }
                }}
            }
        });
        
        receiptDocument.Blocks.Add(new Table
        {
            CellSpacing = 0,
            Columns =
            {
                new TableColumn() { Width = new GridLength(1, GridUnitType.Auto) },
                new TableColumn() { Width = new GridLength(1, GridUnitType.Auto) }
            }, RowGroups =
            {
                new TableRowGroup{Rows =
                {
                    new TableRow
                    {
                        Cells =
                        {
                            new TableCell(new Paragraph(new Run("Vásárlás időpontja:\nDate of purchase"))){TextAlignment = TextAlignment.Left, FontSize = 14},
                            new TableCell(new Paragraph(new Run(DateTime.Now.ToString("yyyy.MM.dd HH:mm:ss")))){TextAlignment = TextAlignment.Right, FontSize = 14}
                        }
                    }
                }}
            }
        });

        receiptDocument.Blocks.Add(new Table
        {
            CellSpacing = 0,
            Columns =
            {
                new TableColumn() { Width = new GridLength(1, GridUnitType.Auto) },
                new TableColumn() { Width = new GridLength(1, GridUnitType.Auto) }
            }, RowGroups =
            {
                new TableRowGroup{Rows =
                {
                    new TableRow
                    {
                        Cells =
                        {
                            new TableCell(new Paragraph(new Bold(new Run("Érvényesség kezdete:\nStart of validity"))){TextAlignment = TextAlignment.Left, FontSize = 14}),
                            new TableCell(new Paragraph(new Bold(new Run("2024.10.16 10:15:01"))){TextAlignment = TextAlignment.Right, FontSize = 14})
                        }
                    }
                }}
            }
        });
        
        receiptDocument.Blocks.Add(new Table
        {
            CellSpacing = 0,
            Columns =
            {
                new TableColumn() { Width = new GridLength(1, GridUnitType.Auto) },
                new TableColumn() { Width = new GridLength(1, GridUnitType.Auto) }
            }, RowGroups =
            {
                new TableRowGroup{Rows =
                {
                    new TableRow
                    {
                        Cells =
                        {
                            new TableCell(new Paragraph(new Run("Érvényesség vége:\nEnd of validity"))){TextAlignment = TextAlignment.Left, FontSize = 14},
                            new TableCell(new Paragraph(new Run("2024.10.16 23:59:59"))){TextAlignment = TextAlignment.Right, FontSize = 14}
                        }
                    }
                }}
            }
        });
        
        receiptDocument.Blocks.Add(new Table
        {
            CellSpacing = 0,
            Columns =
            {
                new TableColumn() { Width = new GridLength(1, GridUnitType.Auto) },
                new TableColumn() { Width = new GridLength(1, GridUnitType.Auto) }
            }, RowGroups =
            {
                new TableRowGroup{Rows =
                {
                    new TableRow
                    {
                        Cells =
                        {
                            new TableCell(new Paragraph(new Bold(new Run("Azonosító:\nUniqe ID")))){TextAlignment = TextAlignment.Left, FontSize = 14},
                            new TableCell(new Paragraph(new Bold(new Run("402408081-\n129033019870")))){TextAlignment = TextAlignment.Right, FontSize = 14}
                        }
                    }
                }}
            }
        });
        
        receiptDocument.Blocks.Add(new Table
        {
            CellSpacing = 0,
            Columns =
            {
                new TableColumn() { Width = new GridLength(1, GridUnitType.Auto) },
                new TableColumn() { Width = new GridLength(1, GridUnitType.Auto) }
            }, RowGroups =
            {
                new TableRowGroup{Rows =
                {
                    new TableRow
                    {
                        Cells =
                        {
                            new TableCell(new Paragraph(new Bold(new Run("Forgalmi rendszám:\nLicense plate number")))){TextAlignment = TextAlignment.Left, FontSize = 14},
                            new TableCell(new Paragraph(new Bold(new Run("ZSG-255")))){TextAlignment = TextAlignment.Right, FontSize = 14}
                        }
                    }
                }}
            }
        });
        
        receiptDocument.Blocks.Add(new Table
        {
            CellSpacing = 0,
            Columns =
            {
                new TableColumn() { Width = new GridLength(1, GridUnitType.Auto) },
                new TableColumn() { Width = new GridLength(1, GridUnitType.Auto) }
            }, RowGroups =
            {
                new TableRowGroup{Rows =
                {
                    new TableRow
                    {
                        Cells =
                        {
                            new TableCell(new Paragraph(new Bold(new Run("Felségjelzés:\nCountry code")))){TextAlignment = TextAlignment.Left, FontSize = 14},
                            new TableCell(new Paragraph(new Bold(new Run("H - Magyarország")))){TextAlignment = TextAlignment.Right, FontSize = 14}
                        }
                    }
                }}
            }
        });
        
        receiptDocument.Blocks.Add(new Table
        {
            CellSpacing = 0,
            Columns =
            {
                new TableColumn() { Width = new GridLength(1, GridUnitType.Auto) },
                new TableColumn() { Width = new GridLength(1, GridUnitType.Auto) }
            }, RowGroups =
            {
                new TableRowGroup{Rows =
                {
                    new TableRow
                    {
                        Cells =
                        {
                            new TableCell(new Paragraph(new Run("Típus/Type"))){TextAlignment = TextAlignment.Left, FontSize = 14},
                            new TableCell(new Paragraph(new Run("Napi (országos)"))){TextAlignment = TextAlignment.Right, FontSize = 14}
                        }
                    }
                }}
            }
        });
        
        receiptDocument.Blocks.Add(new Table
        {
            CellSpacing = 0,
            Columns =
            {
                new TableColumn() { Width = new GridLength(1, GridUnitType.Auto) },
                new TableColumn() { Width = new GridLength(1, GridUnitType.Auto) }
            }, RowGroups =
            {
                new TableRowGroup{Rows =
                {
                    new TableRow
                    {
                        Cells =
                        {
                            new TableCell(new Paragraph(new Bold(new Run("Kategória/Category:")))){TextAlignment = TextAlignment.Left, FontSize = 14},
                            new TableCell(new Paragraph(new Bold(new Run("D1M")))){TextAlignment = TextAlignment.Right, FontSize = 14}
                        }
                    }
                }}
            }
        });
        
        receiptDocument.Blocks.Add(new Table
        {
            CellSpacing = 0,
            Columns =
            {
                new TableColumn() { Width = new GridLength(1, GridUnitType.Auto) },
                new TableColumn() { Width = new GridLength(1, GridUnitType.Auto) }
            }, RowGroups =
            {
                new TableRowGroup{Rows =
                {
                    new TableRow
                    {
                        Cells =
                        {
                            new TableCell(new Paragraph(new Run("Termék:\nProduct"))){TextAlignment = TextAlignment.Left, FontSize = 14},
                            new TableCell(new Paragraph(new Run("NAPI ORSZÁGOS D1M"))){TextAlignment = TextAlignment.Right, FontSize = 14}
                        }
                    }
                }}
            }
        });
        
        receiptDocument.Blocks.Add(new Table
        {
            CellSpacing = 0,
            Columns =
            {
                new TableColumn() { Width = new GridLength(1, GridUnitType.Auto) },
                new TableColumn() { Width = new GridLength(1, GridUnitType.Auto) }
            }, RowGroups =
            {
                new TableRowGroup{Rows =
                {
                    new TableRow
                    {
                        Cells =
                        {
                            new TableCell(new Paragraph(new Bold(new Run("NÚSZ Tr. azonosító:\nNÚSZ Tr. ID")))){TextAlignment = TextAlignment.Left, FontSize = 14},
                            new TableCell(new Paragraph(new Bold(new Run("3N3K7A2T452")))){TextAlignment = TextAlignment.Right, FontSize = 14}
                        }
                    }
                }}
            }
        });
        
        receiptDocument.Blocks.Add(new Paragraph
        {
            TextAlignment = TextAlignment.Center,
            Inlines = { new Bold(new Run("Ár / Price: 2570 HUF"){ FontSize = 18 })}
        });
        
        receiptDocument.Blocks.Add(new Paragraph
        {
            TextAlignment = TextAlignment.Center,
            Inlines = { new Run("\n\n\n-----------------------------------------\nÜgyfél aláírása / Customer's signature"){ FontSize = 14 }}
        });
        
        receiptDocument.Blocks.Add(new Paragraph
        {
            TextAlignment = TextAlignment.Center,
            Inlines = { new Run("Aláírásommal elismerem a fenti adatok helyességét. / I acknowledge the correctness of the above data with my signature."){ FontSize = 14 }}
        });
        
        receiptDocument.Blocks.Add(new Paragraph
        {
            TextAlignment = TextAlignment.Center,
            Inlines = {new Run("Eladói példány, az e-matrica megvásárlását nem igazolja, úthasználatra nem jogosít. / Seller's copy, does not certify the purchase of the e-vignette, does not authorise for road use."){ FontSize = 14 }}
        });
        
        receiptDocument.Blocks.Add(new Paragraph
        {
            TextAlignment = TextAlignment.Center,
            Inlines = { new Bold(new Run("Kérjük a bizonylatot a lejárat után 2 évig megőrizni! / Keep it for 2 years after expiration!\n\n"){ FontSize = 14 })}
        });
        
        IDocumentPaginatorSource paginatorSource = receiptDocument;

        XpsDocumentWriter xpsWriter = PrintQueue.CreateXpsDocumentWriter(printQueue);
        xpsWriter.Write(paginatorSource.DocumentPaginator);
    }
}