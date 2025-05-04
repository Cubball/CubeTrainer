import { ReactNode, useState } from 'react'

export interface TabsProps {
  tabs: {
    name: string
    element: ReactNode
  }[]
}

const Tabs = ({ tabs }: TabsProps) => {
  if (tabs.length === 0) {
    return null
  }

  const [selectedTabIndex, setSelectedTabIndex] = useState(0)
  const selectedTab = tabs[selectedTabIndex] ?? tabs[0]

  return (
    <div className="flex flex-col items-center gap-4">
      <div className="flex gap-4">
        {tabs.map((tab, index) => (
          <button
            key={tab.name}
            className={`${
              selectedTabIndex === index
                ? 'border-b-2 border-gray-800 font-semibold text-gray-800'
                : 'text-gray-500'
            } cursor-pointer px-4 py-2`}
            onClick={() => setSelectedTabIndex(index)}
          >
            {tab.name}
          </button>
        ))}
      </div>
      {selectedTab.element}
    </div>
  )
}

export default Tabs
