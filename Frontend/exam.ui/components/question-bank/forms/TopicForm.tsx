"use client";

import { useState } from "react";
import { FormValidation } from "@/schemas/form-validation";
import { createTopicRequestSchema } from "@/schemas/requests/create-topic-request";

export type TopicFormProps = {
  onSubmit: (topicName: string) => void;
  onCancel: () => void;
  initialValue?: string;
};

export default function TopicForm({ onSubmit, onCancel, initialValue = "" }: TopicFormProps) {
  const [topicName, setTopicName] = useState(initialValue);
  const [topicNameError, setTopicNameError] = useState("");

  function handleSubmit(event:React.SyntheticEvent<HTMLFormElement>) {
    event.preventDefault();
    const result = FormValidation(createTopicRequestSchema, {
      title: topicName,
    });

    if (!result.success) {
      setTopicNameError(
        result.errors.title ?? "Topic name is invalid."
      );
      return;
    }

    setTopicNameError("");
    onSubmit(result.data.title);
    setTopicName("");
  }

  return (
    <form onSubmit={handleSubmit}>
        <div className="flex flex-col gap-1.5">
          <label className="text-label">Topic Name</label>
          <input
            value={topicName}
            onChange={(event) => {
              setTopicName(event.target.value);
              setTopicNameError("");
            }}
            className="input"
            placeholder="Enter topic name"
            autoFocus
          />
        </div>
        

        {topicNameError ? (
          <p className="mt-1 text-error">
            {topicNameError}
          </p>
        ) : null}
      <div className="flex justify-end gap-3 border-t border-slate-100 pt-5">
        <button
          type="button"
          onClick={onCancel}
          className="btn-secondary"
        >
          Cancel
        </button>
        <button
          type="submit"
          className="btn-primary"
        >
          Save
        </button>
      </div>
    </form>
  );
}
