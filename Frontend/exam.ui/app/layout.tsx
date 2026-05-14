import type { Metadata } from "next"; /*Type definition for metadata, used to define the page's title and description for SEO and browser display purposes*/
import { Geist, Geist_Mono } from "next/font/google"; /*Google Fonts loaded using Next.js built-in font optimization*/
import { Toaster } from "sonner";
import "./globals.css";

const geistSans = Geist({
  variable: "--font-geist-sans",
  subsets: ["latin"],
});

const geistMono = Geist_Mono({
  variable: "--font-geist-mono", /*CSS variable name for the mono font, allowing it to be used in stylesheets*/
  subsets: ["latin"], /*only Latin characters are loaded (smaller bundle size)*/
});

/*controls Browser tab title, Meta description (used by Google / SEO)*/
export const metadata: Metadata = {
  title: "Exam Platform",
  description: "Admin dashboard for managing exams and candidates.",
};

/*main wrapper for the application, defines the HTML structure and applies global styles and fonts*/
export default function RootLayout({
  children,
}: Readonly<{
  children: React.ReactNode;
}>) {
  return (
    <html
      lang="en"
      className={`${geistSans.variable} ${geistMono.variable} h-full antialiased`} /*applies the loaded fonts and some global styles (full height, antialiased text) to the entire document*/
    >
      <body className="min-h-full flex flex-col">
        {children}
        <Toaster position="top-right" richColors />
      </body>
    </html>
  );
}
