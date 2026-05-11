"use client";

import { useState } from "react";

export type TopicFormProps = {
  onSubmit: (topicName: string) => void;
  onCancel: () => void;
};

export default function TopicForm({ onSubmit, onCancel }: TopicFormProps) {
  const [topicName, setTopicName] = useState("");
  const [topicNameError, setTopicNameError] = useState("");

  function handleSubmit(event:React.SyntheticEvent<HTMLFormElement>) {
    event.preventDefault();
    const trimmedTopic = topicName.trim();

    if (!trimmedTopic) {
      setTopicNameError("Topic name is required.");
      return;
    }

    setTopicNameError("");
    onSubmit(trimmedTopic);
    setTopicName("");
  }

  return (
    <form onSubmit={handleSubmit} className="space-y-6">
      <label className="block">
        <span className="text-sm font-semibold text-slate-900">Topic Name</span>
        <input
          value={topicName}
          onChange={(event) => {
            setTopicName(event.target.value);
            setTopicNameError("");
          }}
          className="mt-2 h-11 w-full rounded-lg border border-slate-200 px-4 text-sm text-slate-900 outline-none transition placeholder:text-slate-400 focus:border-blue-500 focus:ring-2 focus:ring-blue-100"
          placeholder="Enter topic name"
          autoFocus
        />

        {topicNameError ? (
          <p className="mt-2 text-sm text-red-600">
            {topicNameError}
          </p>
        ) : null}
      </label>
      <div className="flex justify-end gap-3 border-t border-slate-100 pt-5">
        <button
          type="button"
          onClick={onCancel}
          className="bg-white text-black font-semibold px-6 py-2.5 rounded-full hover:brightness-90 transition"
        >
          Cancel
        </button>
        <button
          type="submit"
          className="bg-primary text-white font-semibold px-6 py-2.5 rounded-full hover:brightness-90 transition"
        >
          Save Topic
        </button>
      </div>
    </form>
  );
}
