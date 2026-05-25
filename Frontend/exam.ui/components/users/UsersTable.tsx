"use client";

import SearchBar from "@/components/question-bank/SearchInput";
import { SystemUser } from "@/types/user";

interface Props {
  users: SystemUser[];
  search: string;
  onSearchChange: (value: string) => void;
}

const roleStyle: Record<SystemUser["role"], { bg: string; color: string }> = {
  Owner: { bg: "#fef3c7", color: "#92400e" },
  Admin: { bg: "#eff6ff", color: "#1d4ed8" },
};

export default function UsersTable({
  users,
  search,
  onSearchChange,
}: Props) {
  return (
    <div className="card overflow-hidden">
      <div className="flex gap-3 items-center justify-between px-5 py-4 border-b border-gray-100">
        <SearchBar
          value={search}
          onChange={onSearchChange}
          placeholder="Search users..."
        />
      </div>

      <table className="w-full border-collapse">
        <thead>
          <tr className="border-b border-gray-100">
            {["NAME", "EMAIL", "ROLE"].map((heading) => (
              <th key={heading} className="nav-section-label text-gray-400 px-5 py-3">
                {heading}
              </th>
            ))}
          </tr>
        </thead>
        <tbody>
          {users.length === 0 && (
            <tr>
              <td colSpan={3} className="text-muted text-center py-8">
                [ No users found ]
              </td>
            </tr>
          )}

          {users.map((user, index) => (
            <tr
              key={user.id}
              className={`${index !== users.length - 1 ? "border-b border-gray-50" : ""}`}
            >
              <td className="px-5 py-4 text-label text-center">
                {user.firstName} {user.lastName}
              </td>
              <td className="px-5 py-4 text-muted text-center">{user.email}</td>
              <td className="px-5 py-4 text-center">
                <span
                  className="inline-block px-3 py-1 rounded-full text-[13px] font-medium"
                  style={{
                    background: roleStyle[user.role].bg,
                    color: roleStyle[user.role].color,
                  }}
                >
                  {user.role}
                </span>
              </td>
            </tr>
          ))}
        </tbody>
      </table>
    </div>
  );
}
