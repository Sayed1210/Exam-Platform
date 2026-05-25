"use client";

import DashboardPageHeader from "@/components/common/DashboardHeader";
import AddAdminModal from "@/components/users/AddAdminModal";
import UsersTable from "@/components/users/UsersTable";
import { createAdmin, getUsers } from "@/services/userService";
import { CreateAdminRequest, SystemUser } from "@/types/user";
import { useRouter } from "next/navigation";
import { useEffect, useMemo, useState } from "react";
import { toast } from "sonner";

export default function UsersPage() {
  const router = useRouter();
  const [users, setUsers] = useState<SystemUser[]>([]);
  const [search, setSearch] = useState("");
  const [showAddModal, setShowAddModal] = useState(false);

  const fetchUsers = async () => {
    const result = await getUsers();
    setUsers(result);
  };

  useEffect(() => {
    if (localStorage.getItem("role") !== "Owner") {
      router.replace("/candidates");
      return;
    }

    fetchUsers().catch((error) => {
      toast.error(error?.message || "Failed to load users");
    });
  }, [router]);

  const filteredUsers = useMemo(() => {
    const normalizedSearch = search.trim().toLowerCase();

    if (!normalizedSearch) return users;

    return users.filter((user) => {
      const fullName = `${user.firstName} ${user.lastName}`.toLowerCase();
      return (
        fullName.includes(normalizedSearch) ||
        user.email.toLowerCase().includes(normalizedSearch) ||
        user.role.toLowerCase().includes(normalizedSearch)
      );
    });
  }, [search, users]);

  const handleAddAdmin = async (data: CreateAdminRequest) => {
    try {
      const createdUser = await createAdmin(data);
      setUsers((prev) => [...prev, createdUser]);
      setShowAddModal(false);
      toast.success("Admin added successfully");
    } catch (error: any) {
      toast.error(error?.message || "Failed to add admin");
    }
  };

  return (
    <div>
      <DashboardPageHeader
        title="Users"
        buttonText="+ Add Admin"
        onButtonClick={() => setShowAddModal(true)}
      />

      <UsersTable
        users={filteredUsers}
        search={search}
        onSearchChange={setSearch}
      />

      {showAddModal && (
        <AddAdminModal
          onClose={() => setShowAddModal(false)}
          onSubmit={handleAddAdmin}
        />
      )}
    </div>
  );
}
