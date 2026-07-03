---
name: Bug report
description: Report a bug or unexpected behavior
title: "[Bug] "
labels: bug
body:
  - type: textarea
    id: summary
    attributes:
      label: Summary
      description: What happened?
      placeholder: Describe the issue briefly.
    validations:
      required: true
  - type: textarea
    id: steps
    attributes:
      label: Steps to reproduce
      description: How can the issue be reproduced?
      placeholder: 1. Go to...
    validations:
      required: true
  - type: textarea
    id: expected
    attributes:
      label: Expected behavior
      description: What should happen instead?
    validations:
      required: true
  - type: textarea
    id: env
    attributes:
      label: Environment
      description: Relevant environment, version, or deployment context.
