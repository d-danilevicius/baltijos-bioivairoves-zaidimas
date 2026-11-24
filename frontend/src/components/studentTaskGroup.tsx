import { taskTypeLabels } from '../helpers/gbifTaskMessage'
import { groupLabel } from '../helpers/groupLabel'
import { groupPointsText, lockedGroupMessage, studentStatusLabels, taskPointsText } from '../helpers/studentTaskMessage'
import type { StudentTaskGroup as StudentTaskGroupData } from '../types/studentTask'

type Props = {
  group: StudentTaskGroupData
}

export default function StudentTaskGroup({ group }: Props) {
  const headingId = `group-${group.group}`

  return (
    <section aria-labelledby={headingId} className="mb-4">
      <h2 id={headingId} className="mb-1 text-lg font-semibold">
        {groupLabel(group.group)}
      </h2>
      <p className="mb-1">{groupPointsText(group.points, group.maxPoints)}</p>
      {!group.isUnlocked && <p className="mb-2">{lockedGroupMessage}</p>}
      <ul>
        {group.tasks.map((task) => (
          <li key={task.id} className="flex flex-wrap gap-x-4 py-1">
            <span>{taskTypeLabels[task.type]}</span>
            <span>{task.label}</span>
            <span>{studentStatusLabels[task.status]}</span>
            {task.points !== null && <span>{taskPointsText(task.points)}</span>}
          </li>
        ))}
      </ul>
    </section>
  )
}
