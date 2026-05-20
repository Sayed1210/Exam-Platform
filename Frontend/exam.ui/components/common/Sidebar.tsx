"use client";

import Link from "next/link";
import Image from "next/image";
import { usePathname, useRouter } from "next/navigation";
import { logout } from "@/services/auth-service";
import { ArrowRightStartOnRectangleIcon, DocumentTextIcon, QuestionMarkCircleIcon, UserGroupIcon } from "@heroicons/react/24/outline";

const navItems = [
  {
    label: "Candidates",
    href: "/candidates",
    icon: <UserGroupIcon className="w-[18px] h-[18px]" />
  },
  {
    label: "Exams",
    href: "/exams",
    icon: <DocumentTextIcon className="w-[18px] h-[18px]" />,
  },
  {
    label: "Question Bank",
    href: "/question-bank",
    icon: <QuestionMarkCircleIcon className="w-[18px] h-[18px]" />,
  },
];

export default function Sidebar() {
  const pathname = usePathname();
  const router = useRouter();

  const handleLogout = () => {
    logout();
    localStorage.clear();
    sessionStorage.clear();
    router.replace("/login");
  };

  return (
    <aside className="w-60 min-w-60 bg-foreground flex flex-col h-screen sticky top-0 px-3 py-5">
      
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
          <span className="text-sm font-bold text-background">Enozom HR</span>
          <span className="nav-section-label">Admin Portal</span>
        </div>
      </div>

      <div className="h-px bg-gray-800 mx-2 mb-4" />

      {/* Nav */}
      <nav className="flex-1">
        <p className="nav-section-label px-2">MAIN</p>
        <ul className="flex flex-col gap-1">
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
          className="flex items-center gap-2 w-full px-3 py-2.5 rounded-lg text-sm font-medium text-primary hover:bg-gray-800 transition-colors duration-150"
        >
          <ArrowRightStartOnRectangleIcon className="w-[18px] h-[18px]" />
          Logout
        </button>
      </div>
    </aside>
  );
}
