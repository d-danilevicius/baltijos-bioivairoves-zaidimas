import Button from './button'
import { groupColumn, labelColumn, taskTypeLabels, typeColumn } from '../helpers/gbifTaskMessage'
import { actionsColumn, deleteTaskButton } from '../helpers/teacherTaskMessage'
import type { ClassTask } from '../types/classTask'

type Props = {
  tasks: ClassTask[]
  onDelete: (taskId: string) => void
}

// Only a task created by the teacher can be deleted (FR-034)
function canDelete(task: ClassTask): boolean {
  return task.type === 'TeacherTask'
}

export default function ClassTaskTable({ tasks, onDelete }: Props) {
  return (
    <table className="mb-4 w-full text-left">
      <thead>
        <tr>
          <th className="pb-2">{typeColumn}</th>
          <th className="pb-2">{groupColumn}</th>
          <th className="pb-2">{labelColumn}</th>
          <th className="pb-2">
            <span className="sr-only">{actionsColumn}</span>
          </th>
        </tr>
      </thead>
      <tbody>
        {tasks.map((task) => (
          <tr key={task.id}>
            <td className="py-1 pr-2">{taskTypeLabels[task.type]}</td>
            <td className="py-1 pr-2">{task.difficultyGroup}</td>
            <td className="py-1 pr-2">{task.label}</td>
            <td className="py-1">
              {canDelete(task) && (
                <Button type="button" variant="secondary" onClick={() => onDelete(task.id)}>
                  {deleteTaskButton}
                </Button>
              )}
            </td>
          </tr>
        ))}
      </tbody>
    </table>
  )
}
