"use client";

import Link from "next/link";
import Image from "next/image";
import { usePathname, useRouter } from "next/navigation";
import { logout } from "@/services/auth-service";

const navItems = [
  {
    label: "Candidates",
    href: "/candidates",
    icon: (
      <svg width="18" height="18" fill="none" viewBox="0 0 24 24" stroke="currentColor" strokeWidth="1.8">
        <path d="M17 21v-2a4 4 0 0 0-4-4H5a4 4 0 0 0-4 4v2" />
        <circle cx="9" cy="7" r="4" />
        <path d="M23 21v-2a4 4 0 0 0-3-3.87" />
        <path d="M16 3.13a4 4 0 0 1 0 7.75" />
      </svg>
    ),
  },
  {
    label: "Exams",
    href: "/exams",
    icon: (
      <svg width="18" height="18" fill="none" viewBox="0 0 24 24" stroke="currentColor" strokeWidth="1.8">
        <rect x="3" y="3" width="18" height="18" rx="2" />
        <path d="M9 9h6M9 12h6M9 15h4" />
      </svg>
    ),
  },
  {
    label: "Question Bank",
    href: "/question-bank",
    icon: (
      <svg width="18" height="18" fill="none" viewBox="0 0 24 24" stroke="currentColor" strokeWidth="1.8">
        <circle cx="12" cy="12" r="10" />
        <path d="M9.09 9a3 3 0 0 1 5.83 1c0 2-3 3-3 3" />
        <circle cx="12" cy="17" r="0.5" fill="currentColor" />
      </svg>
    ),
  },
];

export default function Sidebar() {
  const pathname = usePathname();
  const router = useRouter();

  const handleLogout = () => {
    localStorage.clear();
    sessionStorage.clear();
    logout();
    router.replace("/login");
    router.refresh();
  };

  return (
    <aside className="w-60 min-w-60 bg-[#0f1117] flex flex-col h-screen sticky top-0 px-3 py-5">
      
      {/* Brand */}
      <div className="flex items-center gap-3 px-2 pb-4">
     <Image
  src="/images/enozom-logo.png"
  alt="Enozom HR"
  width={40}
  height={40}
  className="rounded-lg"
/>
        <div className="flex flex-col">
          <span className="text-sm font-bold text-gray-100">Enozom HR</span>
          <span className="text-xs text-gray-500">Admin Portal</span>
        </div>
      </div>

      <div className="h-px bg-gray-800 mx-2 mb-4" />

      {/* Nav */}
      <nav className="flex-1">
        <p className="nav-section-label">
          MAIN
        </p>
        <ul className="flex flex-col gap-0.5">
          {navItems.map((item) => {
            const isActive = pathname.startsWith(item.href);
            return (
              <li key={item.href}>
                <Link
                  href={item.href}
                  className={`flex items-center gap-2.5 px-3 py-2.5 rounded-lg text-sm font-medium transition-colors duration-150
                    ${isActive
                      ? "bg-[var(--primary)] text-white"
                      : "text-gray-400 hover:bg-gray-800 hover:text-gray-100"
                    }`}
                >
                  <span className="shrink-0">{item.icon}</span>
                  <span>{item.label}</span>
                </Link>
              </li>
            );
          })}
        </ul>
      </nav>

      {/* Logout */}
      <div className="pt-3 border-t border-gray-800">
     <button
    onClick={handleLogout}
    className="flex items-center gap-2.5 w-full px-3 py-2.5 rounded-lg text-sm font-medium text-red-500 hover:bg-gray-800 transition-colors duration-150"
  >
    <svg
      width="18"
      height="18"
      fill="none"
      viewBox="0 0 24 24"
      stroke="currentColor"
      strokeWidth="1.8"
    >
      <path d="M9 21H5a2 2 0 0 1-2-2V5a2 2 0 0 1 2-2h4" />
      <polyline points="16 17 21 12 16 7" />
      <line x1="21" y1="12" x2="9" y2="12" />
    </svg>

    Logout
  </button>
      </div>

    </aside>
  );
}
