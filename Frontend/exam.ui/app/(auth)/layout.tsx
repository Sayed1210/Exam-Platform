// app/(auth)/layout.tsx

import Image from "next/image";
import { ReactNode } from "react";

export default function AuthLayout({
  children,
}: {
  children: ReactNode;
}) {
  return (
    <div className="min-h-screen flex items-center justify-center bg-background">
      <div className="w-fit max-w-md bg-white rounded-2xl shadow-xl p-8 text-center flex flex-col items-center">
        <Image
          src="/images/enozom-logo.png"
          alt="logo"
          width={100}
          height={100}
        />

        {children}
      </div>
    </div>
  );
}