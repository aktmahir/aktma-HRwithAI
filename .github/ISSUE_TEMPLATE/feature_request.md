---
name: Feature request
description: Suggest an improvement or new capability
title: "[Feature] "
labels: enhancement
body:
  - type: textarea
    id: problem
    attributes:
      label: Problem statement
      description: What problem are you trying to solve?
    validations:
      required: true
  - type: textarea
    id: proposal
    attributes:
      label: Proposed solution
      description: Describe the proposed capability or change.
    validations:
      required: true
  - type: textarea
    id: impact
    attributes:
      label: Expected impact
      description: Who benefits and why does it matter?
