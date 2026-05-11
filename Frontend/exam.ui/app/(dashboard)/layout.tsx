// app/(dashboard)/layout.tsx

export default function DashboardLayout({
  children,
}: {
  children: React.ReactNode;
}) {
  return (
    <div className="flex min-h-screen">
      {/* 
         FUTURE STEP: 
         This is the perfect place to put your <Sidebar /> component 
      */}
      <div className="flex-1 flex flex-col bg-background"> {/* Uses your white bg */}
        {/* 
           FUTURE STEP: 
           This is where your <Navbar /> would go 
        */}
        <main className="flex-1">
          {children} 
        </main>
      </div>
    </div>
  );
}