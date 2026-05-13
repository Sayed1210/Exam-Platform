import { ZodType, z } from "zod";

export function FormValidation<T>(
  schema: ZodType<T>,
  data: unknown
): { success: true; data: T } | { success: false; errors: Partial<Record<keyof T, string>> } {
  const result = schema.safeParse(data);

  if (result.success) {
    return { success: true, data: result.data };
  }

  const errors = Object.fromEntries(
    Object.entries(z.flattenError(result.error).fieldErrors).map(([key, messages]) => [
      key,
      (messages as string[])[0],
    ])
  ) as Partial<Record<keyof T, string>>;

  return { success: false, errors };
}